#if Windows
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebWindowNetCore.Windows;

[ComVisible(true)]
public class Callback(WebViewForm parent)
{
    public Task<bool> DragStart(string fileList)
    {
        var flt = JsonSerializer.Deserialize<FileListType>(fileList, jsonOptions);
        return flt != null
            ? parent.DragStart(flt.Path, flt.FileList)
            : Task.FromResult(false);
    }

    static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

record FileListType(string[] FileList, string Path);

#endif