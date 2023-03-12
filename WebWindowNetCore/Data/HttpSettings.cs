using Microsoft.AspNetCore.Http;

namespace WebWindowNetCore.Data;

public class HttpSettings
{
    public string? ResourceWebroot { get; internal set; }
    public string? WebrootUrl { get; internal set; }
    public string? DefaultHtml { get; internal set; } = "index.html";
    public string? CorsOrigin { get; internal set; }
    public RequestDelegate? SseDelegate { get; internal set; }
    public RequestDelegate? JsonPostDelegate { get; internal set; }
    public int Port { get; internal set; } = 20000;
}
