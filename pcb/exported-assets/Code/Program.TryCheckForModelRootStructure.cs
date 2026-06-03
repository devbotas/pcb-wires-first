namespace KicadModelSync;

internal static partial class Program {
    private static bool TryCheckForModelRootStructure(string path) {
        var isStructureGood = false;

        try {
            var dirName = new DirectoryInfo(path).Name;

            if (dirName.Equals("3dmodels", StringComparison.OrdinalIgnoreCase) || dirName.Equals("packages3d", StringComparison.OrdinalIgnoreCase)) {
                isStructureGood = Directory.EnumerateDirectories(path, "*.3dshapes", SearchOption.TopDirectoryOnly).Any();
            }
        } catch {
            goto error;
        }

        return isStructureGood;

        error:
        return false;
    }
}
