using AspNetExtensions;
using CsTools.Extensions;
using CsTools.Functional;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WebWindowNetCore.Data;

using static CsTools.Core;

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

    public HttpBuilder JsonPost<T, TResult>(string path, Func<T, Task<TResult>> onRequest)
        => this.SideEffect(n => 
                Data.RequestDelegates = Data.RequestDelegates.Append(
                    (WebApplication app) =>
                        app.WithMapPost(path, async (HttpContext context) => 
                            {
                                var param = await context.Request.ReadFromJsonAsync<T>();
                                await context.Response.WriteAsJsonAsync(await onRequest(param!));
                            }))
                            .ToArray());

    public HttpBuilder JsonPost<T, TResult, TE>(string path, Func<T, AsyncResult<TResult, TE>> onRequest)
            where TResult : notnull
            where TE : RequestError
        => this.SideEffect(n =>
                Data.RequestDelegates = Data.RequestDelegates.Append(
                    (WebApplication app) =>
                        app.WithMapPost(path, async context =>
                            {
                                try
                                {
                                    if (context.Request.ContentLength == 0)
                                        await context.Response.WriteAsJsonAsync(Error<TResult, RequestError>(new RequestError(2002, "Wrongly called without parameters")));
                                    else 
                                    {
                                        var param = await context.Request.ReadFromJsonAsync<T>();
                                        await context.Response.WriteAsJsonAsync(await onRequest(param!).ToResult());
                                    }
                                }
                                catch (Exception e)
                                {
                                    await context.Response.WriteAsJsonAsync(Error<TResult, RequestError>(new RequestError(2000, e.Message)));
                                }
                            }))
                            .ToArray());

    public HttpBuilder JsonPost<TResult, TE>(string path, Func<AsyncResult<TResult, TE>> onRequest)
            where TResult : notnull
            where TE : RequestError
        => this.SideEffect(n =>
                Data.RequestDelegates = Data.RequestDelegates.Append(
                    (WebApplication app) =>
                        app.WithMapPost(path, async context =>
                            {
                                try
                                {
                                    if (context.Request.ContentLength != 0)
                                        await context.Response.WriteAsJsonAsync(Error<TResult, RequestError>(new RequestError(2001, "Wrongly called with parameters")));
                                    else
                                        await context.Response.WriteAsJsonAsync(await onRequest().ToResult());
                                }
                                catch (Exception e)
                                {
                                    await context.Response.WriteAsJsonAsync(Error<TResult, RequestError>(new RequestError(2000, e.Message)));
                                }
                            }))
                            .ToArray());

    public HttpBuilder MapGet(string pattern, RequestDelegate requestDelegate)
        => this.SideEffect(n => 
                Data.RequestDelegates = Data.RequestDelegates.Append(
                    (WebApplication app) =>
                        app.WithMapGet(pattern, requestDelegate))
                            .ToArray());

    public HttpBuilder MapGet(string pattern, Delegate handler)
        => this.SideEffect(n => 
                Data.RequestDelegates = Data.RequestDelegates.Append(
                    (WebApplication app) =>
                        app.WithMapGet(pattern, handler))
                            .ToArray());

    public HttpBuilder UseReverseProxy(string host, string pattern, string reverseUrl)
        => this.SideEffect(n => 
                Data.RequestDelegates = Data.RequestDelegates.Append(
                    (WebApplication app) =>
                        app.WithHost(host)
                            .WithReverseProxy(pattern, reverseUrl)
                            .GetApp())
                            .ToArray());

    public HttpSettings Build() => Data.SideEffect(Kestrel.Start);

    internal HttpSettings Data { get; } = new();

    internal HttpBuilder() { }
}