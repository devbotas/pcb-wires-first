using System.Runtime.InteropServices;

namespace KicadModelSync;

internal static class Program {
    private static int Main(string[] args) {
        try {
            var options = Options.Parse(args);

            if (options.ShowHelp) {
                PrintHelp();
                return 0;
            }

            var sourceDir = Path.GetFullPath(options.SourceDir ??
                                             Path.Combine(Environment.CurrentDirectory, "..\\external-models"));
            var destDir = Path.GetFullPath(options.DestDir ?? DetectKicad3DModelDir() ?? string.Empty);

            if (string.IsNullOrWhiteSpace(destDir)) {
                Console.Error.WriteLine("Could not auto-detect KiCad stock 3D model directory.");
                Console.Error.WriteLine("Pass --dest <path> explicitly.");
                return 2;
            }

            if (!Directory.Exists(sourceDir)) {
                Console.Error.WriteLine($"Source folder does not exist: {sourceDir}");
                return 3;
            }

            if (!Directory.Exists(destDir)) {
                Console.Error.WriteLine($"Destination folder does not exist: {destDir}");
                return 4;
            }

            Console.WriteLine($"Source:      {sourceDir}");
            Console.WriteLine($"Destination: {destDir}");
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
                return 0;
            }

            int copied = 0, overwritten = 0, skipped = 0, failed = 0;

            foreach (var sourceFile in sourceFiles) {
                var relativePath = Path.GetRelativePath(sourceDir, sourceFile);
                var targetFile = Path.Combine(destDir, relativePath);
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

            return failed == 0 ? 0 : 1;
        } catch (Exception ex) {
            Console.Error.WriteLine(ex);
            return 99;
        }
    }

    private static string? DetectKicad3DModelDir() {
        foreach (var candidate in GetDetectionCandidates()) {
            // if (string.IsNullOrWhiteSpace(candidate)) {
            //     continue;
            // }

            try {
                var full = Path.GetFullPath(candidate);

                if (Directory.Exists(full) && LooksLikeKicad3DModelsRoot(full)) {
                    return full;
                }
            } catch {
            }
        }

        return null;
    }

    private static IEnumerable<string> GetDetectionCandidates() {
        var envVarCandidates = new[] {
            "KICAD12_3DMODEL_DIR",
            "KICAD11_3DMODEL_DIR",
            "KICAD10_3DMODEL_DIR"
        };

        foreach (var name in envVarCandidates) {
            var value = Environment.GetEnvironmentVariable(name);

            if (!string.IsNullOrWhiteSpace(value)) {
                yield return value;
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            var programFiles = new[] {
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    Environment.GetEnvironmentVariable("ProgramW6432"),
                    Environment.GetEnvironmentVariable("ProgramFiles(x86)")
                }
                .Where(x => string.IsNullOrWhiteSpace(x)==false)
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var baseDir in programFiles) {
                foreach (var version in new[] { "12.0", "11.0", "10.0" }) {
                    yield return Path.Combine(baseDir!, "KiCad", version, "share", "kicad", "3dmodels");
                }

                yield return Path.Combine(baseDir!, "KiCad", "share", "kicad", "3dmodels");
                yield return Path.Combine(baseDir!, "KiCad", "share", "modules", "packages3d");
            }
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            foreach (var path in new[] {
                         "/usr/share/kicad/3dmodels",
                         "/usr/local/share/kicad/3dmodels",
                         "/usr/lib/kicad/share/kicad/3dmodels",
                         "/usr/lib/x86_64-linux-gnu/kicad/share/kicad/3dmodels",
                         "/usr/lib/kicad-nightly/share/kicad/3dmodels",
                         "/usr/share/kicad/modules/packages3d",
                         "/usr/local/share/kicad/modules/packages3d",
                         "/usr/lib/kicad/share/kicad/modules/packages3d",
                         "/usr/lib/x86_64-linux-gnu/kicad/share/kicad/modules/packages3d",
                         "/usr/lib/kicad-nightly/share/kicad/modules/packages3d"
                     }) {
                yield return path;
            }
        } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
            foreach (var path in new[] {
                         "/Applications/KiCad/KiCad.app/Contents/SharedSupport/3dmodels",
                         "/Applications/KiCad/KiCad.app/Contents/SharedSupport/share/kicad/3dmodels"
                     }) {
                yield return path;
            }
        }
    }

    private static bool LooksLikeKicad3DModelsRoot(string path) {
        try {
            if (!Directory.Exists(path)) {
                return false;
            }

            var dirName = new DirectoryInfo(path).Name;

            if (dirName.Equals("3dmodels", StringComparison.OrdinalIgnoreCase) ||
                dirName.Equals("packages3d", StringComparison.OrdinalIgnoreCase)) {
                var subdirs = Directory.EnumerateDirectories(path, "*.3dshapes", SearchOption.TopDirectoryOnly).Take(3)
                    .ToList();
                if (subdirs.Count > 0) {
                    return true;
                }

                var files = Directory.EnumerateFiles(path, "*.step", SearchOption.AllDirectories).Take(1).Any()
                            || Directory.EnumerateFiles(path, "*.stp", SearchOption.AllDirectories).Take(1).Any()
                            || Directory.EnumerateFiles(path, "*.wrl", SearchOption.AllDirectories).Take(1).Any();

                if (files) {
                    return true;
                }
            }
        } catch {
        }

        return false;
    }

    private static void PrintHelp() {
        Console.WriteLine("KicadModelSync");
        Console.WriteLine();
        Console.WriteLine("Copies project-local 3D models into KiCad stock 3D library folders.");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  KicadModelSync [--source <folder>] [--dest <folder>] [--overwrite]");
        Console.WriteLine();
        Console.WriteLine("Defaults:");
        Console.WriteLine("  --source     ./external-models");
        Console.WriteLine("  --dest       Auto-detect from KiCad environment vars and common install paths");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine(@"  KicadModelSync --source ""./external-models""");
        Console.WriteLine(@"  KicadModelSync --source ""./external-models"" --overwrite");
        Console.WriteLine(@"  KicadModelSync --source ""./external-models"" --dest ""/usr/share/kicad/3dmodels""");
    }

    private sealed class Options {
        public string? SourceDir { get; private set; }
        public string? DestDir { get; private set; }
        public bool Overwrite { get; private set; }
        public bool ShowHelp { get; private set; }

        public static Options Parse(string[] args) {
            var opt = new Options();

            for (int i = 0; i < args.Length; i++) {
                switch (args[i].ToLowerInvariant()) {
                    case "--source":
                    case "-s":
                        opt.SourceDir = RequireValue(args, ref i);
                        break;

                    case "--dest":
                    case "-d":
                        opt.DestDir = RequireValue(args, ref i);
                        break;

                    case "--overwrite":
                    case "-o":
                        opt.Overwrite = true;
                        break;

                    case "--help":
                    case "-h":
                    case "/?":
                        opt.ShowHelp = true;
                        break;

                    default:
                        throw new ArgumentException($"Unknown argument: {args[i]}");
                }
            }

            return opt;
        }

        private static string RequireValue(string[] args, ref int i) {
            if (i + 1 >= args.Length) {
                throw new ArgumentException($"Missing value for {args[i]}");
            }
            i++;
            return args[i];
        }
    }
}
