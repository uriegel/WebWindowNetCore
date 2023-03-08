using LinqExtensions.Extensions;
using WebWindowNetCore.Data;

namespace WebWindowNetCore;

public abstract class WebViewBuilder
{
    public WebViewBuilder InitialBounds(int width, int height)
        => this
                .SideEffect(n => Data.Width = width)
                .SideEffect(n => Data.Height = height);

    public WebViewBuilder Title(string title)
        => this.SideEffect(n => Data.TitleString = title);

    public WebViewBuilder Url(string url)
        => this.SideEffect(n => Data.UrlString = url);

    public WebViewBuilder DebuggingEnabled()
        => this.SideEffect(n => Data.DevTools = true);

    public WebViewBuilder ConfigureHttp(int port = 20000)
        => this.SideEffect(n => Data.HttpBuilder = new HttpBuilder(port));

    public abstract WebView Build();
    
    protected WebViewBuilder() { }

    protected WebViewSettings Data { get; } = new();
}

// TODO load-changed event in gtkDotNet
// TODO run javascript get button, set click handler, show devtools
// TODO save/restore bounds if requested
// TODO host web site in kestrel and resources
// TODO To WebWindowDotNet nuget 0.0.1alpha
// TODO Then to WebWindowDotNet.Linux nuget 0.0.1alpha
// TODO Windows version
// TODO initial webview showing splash screen
// TODO string IconPath = "", 
// TODO bool FullscreenEnabled = false,
// TODO bool SaveWindowSettings = false,