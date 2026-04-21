using System.Diagnostics;
using Microsoft.Maui.Storage;

namespace MokkiSovellus_MAUI.Data;

public static class DatabaseDebug
{
    public static string GetDbPath()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
        Debug.WriteLine($"[DB PATH] {path}");
        return path;
    }

    public static void PrintAllFiles()
    {
        var dir = FileSystem.AppDataDirectory;

        Debug.WriteLine("=== APP DATA DIRECTORY ===");
        Debug.WriteLine(dir);

        var files = Directory.GetFiles(dir);

        Debug.WriteLine("=== FILES ===");
        foreach (var f in files)
        {
            Debug.WriteLine(f);
        }
    }
}