using System.Diagnostics;

namespace WebWindowNetCore.Data;

public class WebViewSettings
{
    public string AppId { get; internal set; } = "de.uriegel.webwindownetcore";
    public int Width { get; internal set; } = 800;
    public int Height { get; internal set; } = 600;
    public string Title { get; internal set; } = "";
    public string? Url { get; internal set; }
    public string? Query { get; internal set; }
    public Func<string>? GetQuery { get; internal set; }
    public string? DebugUrl { get; internal set; }
    public bool SaveBounds { get; internal set; }
    public HttpSettings? HttpSettings { get; internal set; }
    public bool DevTools { get; internal set; }
    public string? ResourceIcon { get; internal set; }
    public bool WithoutNativeTitlebar { get; internal set; }
    public Action<WebWindowState>? OnWindowStateChanged { get; internal set; }
    public Action<string, bool, string[]>? OnFilesDrop { get; internal set; }
    public Action? OnStarted { get; internal set; }
    public bool DefaultContextMenuEnabled { get; internal set; }

    public static string GetUri(WebViewSettings settings)
        => (Debugger.IsAttached && !string.IsNullOrEmpty(settings.DebugUrl)
            ? settings.DebugUrl
            : settings.Url != null
            ? settings.Url
            : $"http://localhost:{settings.HttpSettings?.Port ?? 80}{settings.HttpSettings?.WebrootUrl}/{settings.HttpSettings?.DefaultHtml}")
                + (settings.Query ?? settings.GetQuery?.Invoke());
}

