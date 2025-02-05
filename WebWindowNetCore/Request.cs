using System.Text.Json;
using CsTools.Extensions;

namespace WebWindowNetCore;

public class Request(string cmd, string id, string json)
{
    public string Cmd { get => cmd; }
    public T? Deserialize<T>() => json.Deserialize<T>(Json.Defaults);

    public void Response<T>(T t)
    {
        var back = $"result,{id},{JsonSerializer.Serialize(t, Json.Defaults)}".Replace("'", "u0027");
        Javascript.Run($"console.log('Affe')");
        Javascript.Run($"WebView.backtothefuture('{back}')");
    }

    internal static Request Create(string msg)
    {
        var str = msg[8..];
        var idx = str.IndexOf(',');
        var cmd = str[..idx];
        str = str[(idx+1)..];
        idx = str.IndexOf(',');
        var id = str[..idx];
        var json = str[(idx+1)..];
        return new(cmd, id, json);
    }
}
