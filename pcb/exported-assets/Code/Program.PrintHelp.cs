namespace KicadModelSync;

internal static partial class Program {
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
}
