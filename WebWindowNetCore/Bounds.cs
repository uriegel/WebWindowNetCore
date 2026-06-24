using CsTools;
using CsTools.Extensions;

namespace WebWindowNetCore;

record Bounds(int? X, int? Y, int? Width, int? Height, bool IsMaximized)
{
    public static Bounds Retrieve(string id)
        => GetPath(id)
            .ReadAllTextFromFilePath()
            .SideEffect(Console.WriteLine)
            ?.Deserialize<Bounds>(Json.Defaults) 
            ?? new(null, null, null, null, false);

    public static void Save(string id, Bounds bounds)
        => GetPath(id).WriteAllTextToFilePath(bounds.Serialize(Json.Defaults));

    static string GetPath(string id)
        => Environment
            .GetFolderPath(Environment.SpecialFolder.ApplicationData)
            .AppendPath(id)
            .SideEffect(d => d.EnsureDirectoryExists())
            .AppendPath("bounds.json");
}