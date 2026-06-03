namespace KicadModelSync;

internal static partial class Program {
    private static string? DetectKicad3DModelDir() {
        foreach (var candidate in GetDetectionCandidates()) {
            try {
                var betterCandidate = Path.GetFullPath(candidate);

                if (Directory.Exists(betterCandidate) == false) { continue; }
                if (TryCheckForModelRootStructure(betterCandidate) == false) { continue; }

                return betterCandidate;
            } catch {
                // Swallowing.
            }
        }

        return null;
    }
}
