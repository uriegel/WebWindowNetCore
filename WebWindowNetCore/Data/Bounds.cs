using CsTools;
using CsTools.Extensions;

using static AspNetExtensions.Core;

namespace WebWindowNetCore.Data;

public record Bounds(int? X, int? Y, int? Width, int? Height, bool? IsMaximized)
{
    public static Bounds Retrieve(string id, Bounds? defaultValue = null)
        => GetBoundsPath(id)
            .ReadAllTextFromFilePath()
            ?.Deserialize<Bounds>(JsonWebDefaults)
            ?? defaultValue 
            ?? new(null, null, null, null, null);

    public void Save(string id)
        => GetBoundsPath(id)
            .WriteAllTextToFilePath(this.Serialize(JsonWebDefaults));

    static string GetBoundsPath(string id)
        => Environment
            .GetFolderPath(Environment.SpecialFolder.ApplicationData)
            .AppendPath(id)
            .EnsureDirectoryExists()
            .AppendPath("bounds.json");
}

