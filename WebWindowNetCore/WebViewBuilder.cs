using LinqTools;
using WebWindowNetCore.Data;

namespace WebWindowNetCore;

public abstract class WebViewBuilder
{
    public WebViewBuilder InitialBounds(int width, int height)
        => this
                .SideEffect(n => Data.Width = width)
                .SideEffect(n => Data.Height = height);

    public WebViewBuilder Title(string title)
        => this.SideEffect(n => Data.Title = title);

    public WebViewBuilder Url(string url)
        => this.SideEffect(n => Data.Url = url);

    /// <summary>
    /// This url is set to the webview only in debug mode, if HttpBuilder.ResourceWebroot is normally used. 
    /// It is used for React, Vue,... which have their
    /// own web server at debug time, like http://localhost:3000 . If set, it has precedence over 
    /// HttpBuilder.ResourceWebroot
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public WebViewBuilder DebugUrl(string url)
        => this.SideEffect(n => Data.DebugUrl = url);

    public WebViewBuilder SaveBounds()
        => this.SideEffect(n => Data.SaveBounds = true);

    public WebViewBuilder DebuggingEnabled()
        => this.SideEffect(n => Data.DevTools = true);

    public WebViewBuilder ResourceIcon(string resourcePath)
        => this.SideEffect(n =>  Data.ResourceIcon = resourcePath);

    public WebViewBuilder ConfigureHttp(Func<HttpBuilder, HttpSettings> builder)
        => this.SideEffect(n => Data.HttpSettings = builder(new HttpBuilder()));

    public abstract WebView Build();
    
    protected WebViewBuilder() { }

    protected WebViewSettings Data { get; } = new();
}

// TODO Cors?
// TODO RestApi
// TODO fetch for post api
// TODO showDevtools in linux
// TODO debugUrl in linux
// TODO showDevtools in windows
// TODO debugUrl in windows
// TODO inject d.ts
// TODO DebugUrl in platform dependant projects
// TODO Linux icon
// TODO bool FullscreenEnabled = false,
// TODO File Drag and Drop (Windows)
