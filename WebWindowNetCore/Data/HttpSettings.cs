namespace WebWindowNetCore.Data;

public class HttpSettings
{
    public string? ResourceFavicon { get; internal set; }
    public string? ResourceWebroot { get; internal set; }
    public string? WebrootUri { get; internal set; }
    public string? CorsOrigin { get; internal set; }
    public int Port { get; internal set; } = 20000;
}
