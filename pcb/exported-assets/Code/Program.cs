using System.Runtime.InteropServices;

namespace KicadModelSync;

internal static partial class Program {
    private static int Main(string[] args) {
        try {
            var options = Options.Parse(args);

            if (options.ShowHelp) {
                PrintHelp();
                return 0;
            }

            var sourceDir = Path.GetFullPath(options.SourceDir ?? Environment.CurrentDirectory);
            var destinationDir = Path.GetFullPath(options.DestinationDir ?? DetectKicad3DModelDir() ?? string.Empty);

            if (string.IsNullOrWhiteSpace(destinationDir)) {
                Console.Error.WriteLine("Could not auto-detect KiCad stock 3D model directory.");
                Console.Error.WriteLine("Pass --dest <path> explicitly.");
                return -2;
            }

            if (Directory.Exists(sourceDir) == false) {
                Console.Error.WriteLine($"Source folder does not exist: {sourceDir}");
                return -3;
            }

            if (Directory.Exists(destinationDir) == false) {
                Console.Error.WriteLine($"Destination folder does not exist: {destinationDir}");
                return -4;
            }

            Console.WriteLine($"Source:      {sourceDir}");
            Console.WriteLine($"Destination: {destinationDir}");
            Console.WriteLine();

            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
                ".step", ".stp", ".wrl"
            };

            var sourceFiles = Directory
                .EnumerateFiles(sourceDir, "*.*", SearchOption.AllDirectories)
                .Where(f => allowedExtensions.Contains(Path.GetExtension(f)))
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (sourceFiles.Count == 0) {
                Console.WriteLine("No model files found.");
                return -5;
            }

            int copied = 0, overwritten = 0, skipped = 0, failed = 0;

            foreach (var sourceFile in sourceFiles) {
                var relativePath = Path.GetRelativePath(sourceDir, sourceFile);
                var targetFile = Path.Combine(destinationDir, relativePath);
                var targetFolder = Path.GetDirectoryName(targetFile)!;

                try {
                    Directory.CreateDirectory(targetFolder);

                    if (File.Exists(targetFile)) {
                        if (options.Overwrite) {
                            File.Copy(sourceFile, targetFile, true);
                            Console.WriteLine($"OVERWRITE  {relativePath}");
                            overwritten++;
                        } else {
                            Console.WriteLine($"SKIP       {relativePath}");
                            skipped++;
                        }
                    } else {
                        File.Copy(sourceFile, targetFile, false);
                        Console.WriteLine($"COPY       {relativePath}");
                        copied++;
                    }
                } catch (UnauthorizedAccessException ex) {
                    Console.WriteLine($"FAILED     {relativePath} :: permission denied :: {ex.Message}");
                    failed++;
                } catch (Exception ex) {
                    Console.WriteLine($"FAILED     {relativePath} :: {ex.Message}");
                    failed++;
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Copied:      {copied}");
            Console.WriteLine($"Overwritten: {overwritten}");
            Console.WriteLine($"Skipped:     {skipped}");
            Console.WriteLine($"Failed:      {failed}");

            if (failed > 0) {
                Console.WriteLine();
                Console.WriteLine("Tip: system package paths may require elevated privileges.");
            }

            return failed == 0 ? 0 : -6;
        } catch (Exception ex) {
            Console.Error.WriteLine(ex);
            return -1;
        }
    }
}
