using AspNetExtensions;
using LinqTools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WebWindowNetCore.Data;

namespace WebWindowNetCore;

public class HttpBuilder
{
    public HttpBuilder Port(int port)
        => this.SideEffect(n => Data.Port = port);
    public HttpBuilder CorsOrigin(string origin)
        => this.SideEffect(n => Data.CorsOrigin = origin);

    public HttpBuilder DefaultHtml(string defaultHtml)
        => this.SideEffect(n =>  Data.DefaultHtml = defaultHtml);

    public HttpBuilder ResourceWebroot(string resourcePath, string webrootUrl)
        => this.SideEffect(n => 
        {
            Data.ResourceWebroot = resourcePath;
            Data.WebrootUrl = webrootUrl;
        });

    public HttpBuilder UseSse<T>(string path, SseEventSource<T> sseEventSource)
        => this.SideEffect(n => 
                Data.RequestDelegates = Data.RequestDelegates.Append(
                    (WebApplication app) =>
                        app.WithMapGet(path, (HttpContext context) => new Sse<T>(sseEventSource.Subject).Start(context)))
                            .ToArray());

    public HttpBuilder UseSse<T>(string path, IObservable<T> sseEventSource)
        => this.SideEffect(n => 
                Data.RequestDelegates = Data.RequestDelegates.Append(
                    (WebApplication app) =>
                        app.WithMapGet(path, (HttpContext context) => new Sse<T>(sseEventSource).Start(context)))
                            .ToArray());

    public HttpBuilder UseJsonPost<T, TResult>(string path, Func<T, Task<TResult>> onRequest)
        => this.SideEffect(n => 
                Data.RequestDelegates = Data.RequestDelegates.Append(
                    (WebApplication app) =>
                        app.WithMapPost(path, async (HttpContext context) => 
                            {
                                var param = await context.Request.ReadFromJsonAsync<T>();
                                await context.Response.WriteAsJsonAsync<TResult>(await onRequest(param!));
                            }))
                            .ToArray());

    public HttpSettings Build() => Data.SideEffect(Kestrel.Start);

    internal HttpSettings Data { get; } = new();

    internal HttpBuilder() { }
}