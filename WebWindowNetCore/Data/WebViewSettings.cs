namespace WebWindowNetCore.Data;

public class WebViewSettings
{
    public int Width { get; internal set; } = 800;
    public int Height  { get; internal set; } = 600;
    public string Title { get; internal set; } = "";
    public string? Url { get; internal set; }
    public string? Query { get; internal set; }
    public string? DebugUrl { get; internal set; }
    public bool SaveBounds { get; internal set; }
    public HttpSettings? HttpSettings { get; internal set; }
    public bool DevTools { get; internal set; }
    public string? ResourceIcon { get; internal set; }
    public bool WithoutNativeTitlebar { get; internal set; }
    public Action<WebWindowState>? OnWindowStateChanged { get; internal set; }
}