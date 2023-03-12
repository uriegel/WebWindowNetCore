using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebWindowNetCore.Data;

public class HttpSettings
{
    public string? ResourceWebroot { get; internal set; }
    public string? WebrootUrl { get; internal set; }
    public string? DefaultHtml { get; internal set; } = "index.html";
    public string? CorsOrigin { get; internal set; }
    public Func<WebApplication, WebApplication>[] RequestDelegates { get; internal set; } = Array.Empty<Func<WebApplication, WebApplication>>();
    public int Port { get; internal set; } = 20000;
}

public record Sse(string Path, RequestDelegate SseDelegate);