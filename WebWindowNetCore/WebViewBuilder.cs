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

    public WebViewBuilder ResourceFavicon(string path)
        => this.SideEffect(n => Data.ResourceFavicon = path);

    public WebViewBuilder ResourceWebroot(string path)
        => this.SideEffect(n => Data.ResourceWebroot = path);

    public WebViewBuilder SaveBounds()
        => this.SideEffect(n => Data.SaveBounds = true);

    public WebViewBuilder DebuggingEnabled()
        => this.SideEffect(n => Data.DevTools = true);

    public WebViewBuilder ConfigureHttp(int port = 20000)
        => this.SideEffect(n => Data.HttpBuilder = new HttpBuilder(port));

    public abstract WebView Build();
    
    protected WebViewBuilder() { }

    protected WebViewSettings Data { get; } = new();
}

// TODO Kestrel
// TODO host web site in kestrel
// TODO bool FullscreenEnabled = false,
// TODO bool SaveWindowSettings = false,
