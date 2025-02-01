using CsTools;
using CsTools.Extensions;

namespace WebWindowNetCore;

record Bounds(int? X, int? Y, int? Width, int? Height, bool IsMaximized)
{
    public static Bounds Retrieve(string id)
        => Environment
            .GetFolderPath(Environment.SpecialFolder.ApplicationData)
            .AppendPath(id)
            .SideEffect(d => d.EnsureDirectoryExists())
            .AppendPath("bounds.json")
            .ReadAllTextFromFilePath()
            .SideEffect(Console.WriteLine)
            ?.Deserialize<Bounds>(Json.Defaults) 
            ?? new(null, null, null, null, false);
}