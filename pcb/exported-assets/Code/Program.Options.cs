using System.Diagnostics.CodeAnalysis;

namespace KicadModelSync;

sealed class Options {
    public string? SourceDir { get; private set; }
    public string? DestinationDir { get; private set; }
    public bool Overwrite { get; private set; }
    public bool ShowHelp { get; private set; }

    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "It's fine.")]
    public static Options Parse(string[] args) {
        var options = new Options();

        for (var i = 0; i < args.Length; i++) {
            if (args[i] is not { } arg) { continue; }

            switch (arg.ToLowerInvariant()) {
                case "--source":
                case "-s":
                    options.SourceDir = RequireValue(args, ref i);
                    break;

                case "--dest":
                case "-d":
                    options.DestinationDir = RequireValue(args, ref i);
                    break;

                case "--overwrite":
                case "-o":
                    options.Overwrite = true;
                    break;

                case "--help":
                case "-h":
                case "/?":
                    options.ShowHelp = true;
                    break;

                default:
                    throw new ArgumentException($"Unknown argument: {args[i]}");
            }
        }

        return options;
    }

    private static string RequireValue(string[] args, ref int i) {
        if (i + 1 >= args.Length) {
            throw new ArgumentException($"Missing value for {args[i]}");
        }
        i++;
        return args[i];
    }
}
