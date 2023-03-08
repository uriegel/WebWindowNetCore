namespace WebWindowNetCore.Data;

public class WebViewSettings
{
    public int Width { get; internal set; } = 800;
    public int Height  { get; internal set; } = 600;
    public string TitleString { get; internal set; } = "";
    public string? UrlString { get; internal set; }
    public HttpBuilder? HttpBuilder { get; internal set; }
    public bool DevTools { get; internal set; }
}