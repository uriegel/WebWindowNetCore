namespace WebWindowNetCore.Data;

public class HttpSettings
{
    public string? ResourceFavicon { get; internal set; }
    public string? ResourceWebroot { get; internal set; }
    public string? WebrootUri { get; internal set; }
    public string? DefaultHtml { get; internal set; } = "index.html";
    public string? CorsOrigin { get; internal set; }
    public int Port { get; internal set; } = 20000;
}
