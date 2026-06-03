using System.Runtime.InteropServices;

namespace KicadModelSync;

internal static partial class Program {
    /// <summary>
    /// All those folders were vibe-coded. It does not mean all then are possible, but hey, Perplexity looked through ancient KiCad versions, too.
    /// </summary>
    private static List<string> GetDetectionCandidates() {
        var dirsToCheck = new List<string>();

        var envVarCandidates = new[] {
            "KICAD12_3DMODEL_DIR",
            "KICAD11_3DMODEL_DIR",
            "KICAD10_3DMODEL_DIR"
        };

        foreach (var name in envVarCandidates) {
            var value = Environment.GetEnvironmentVariable(name);

            if (string.IsNullOrWhiteSpace(value) == false) {
                dirsToCheck.Add(value);
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            var programFiles = new List<string> {
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    Environment.GetEnvironmentVariable("ProgramW6432") ?? "",
                    Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? ""
                }
                .Where(x => string.IsNullOrWhiteSpace(x) == false)
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var baseDir in programFiles) {
                foreach (var version in new[] { "12.0", "11.0", "10.0" }) {
                    dirsToCheck.Add(Path.Combine(baseDir, "KiCad", version, "share", "kicad", "3dmodels"));
                }

                dirsToCheck.Add(Path.Combine(baseDir!, "KiCad", "share", "kicad", "3dmodels"));
                dirsToCheck.Add(Path.Combine(baseDir!, "KiCad", "share", "modules", "packages3d"));
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            dirsToCheck.AddRange([
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
            ]);
        }

        return dirsToCheck;
    }
}
