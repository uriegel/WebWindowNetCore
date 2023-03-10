using LinqTools;
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

    public HttpBuilder ResourceWebroot(string resourcePath, string webrootUri)
        => this.SideEffect(n => 
        {
            Data.ResourceWebroot = resourcePath;
            Data.WebrootUri = webrootUri;
        });

    public HttpBuilder ResourceFavicon(string resourcePath)
        => this.SideEffect(n =>  Data.ResourceFavicon = resourcePath);

    public HttpSettings Build() => Data.SideEffect(Kestrel.Start);

    internal HttpSettings Data { get; } = new();

    internal HttpBuilder() { }
}