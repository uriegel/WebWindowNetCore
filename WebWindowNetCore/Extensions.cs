using System.Text.Json;
using LinqTools;
using Microsoft.AspNetCore.Builder;

static class Extensions
{
    /// <summary>
    /// Reads all text from a text file
    /// </summary>
    /// <param name="path">Full path to the text file</param>
    /// <returns>Containing text</returns>
    public static string? ReadAllTextFromFilePath1(this string path)
        => File.Exists(path)
            ? new StreamReader(File.OpenRead(path))
                .Use(f => f.ReadToEnd())
            : null;

    public static void WriteAllTextToFilePath(this string path, string text)
        => File
            .CreateText(path)
            .Use(str => str.Write(text));

    public static T? Deserialize<T>(this string json, JsonSerializerOptions? options = null) 
        => JsonSerializer.Deserialize<T>(json, options);

    public static string Serialize<T>(this T json, JsonSerializerOptions? options = null) 
        => JsonSerializer.Serialize(json, options);
}
