using LinqTools;
using WebWindowNetCore.Data;

namespace WebWindowNetCore.Base;

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

    /// <summary>
    /// Add a query string to the Url (or DebugUrl, or HttpBuilder.ResourceWebroot)
    /// </summary>
    /// <param name="query">A query string such as '?theme=adwaita&amp;platform=linux' which is added to the Url</param>
    /// <returns></returns>
    public WebViewBuilder QueryString(string query)
        => this.SideEffect(n => Data.Query = query);

    /// <summary>
    /// Add a query string to the Url (or DebugUrl, or HttpBuilder.ResourceWebroot)
    /// </summary>
    /// <param name="getQuery">A function returning a query string such as '?theme=adwaita&amp;platform=linux' which is added to the Url</param>
    /// <returns></returns>
    public WebViewBuilder QueryString(Func<string> getQuery)
        => this.SideEffect(n => Data.GetQuery = getQuery);

    public WebViewBuilder SaveBounds()
        => this.SideEffect(n => Data.SaveBounds = true);

    public WebViewBuilder DebuggingEnabled()
        => this.SideEffect(n => Data.DevTools = true);

    public WebViewBuilder ResourceIcon(string resourcePath)
        => this.SideEffect(n =>  Data.ResourceIcon = resourcePath);

    public WebViewBuilder ConfigureHttp(Func<HttpBuilder, HttpSettings> builder)
        => this.SideEffect(n => Data.HttpSettings = builder(new HttpBuilder()));

    public WebViewBuilder WithoutNativeTitlebar()
        => this.SideEffect(n => Data.WithoutNativeTitlebar = true);

    public WebViewBuilder OnWindowStateChanged(Action<WebWindowState> action)
        => this.SideEffect(n => Data.OnWindowStateChanged = action);        

    public WebViewBuilder OnFilesDrop(Action<string, bool, string[]> onFilesDrop)
        => this.SideEffect(n => Data.OnFilesDrop = onFilesDrop);        

    public WebViewBuilder OnStarted(Action onStarted)
        => this.SideEffect(n => Data.OnStarted = onStarted);        

    public abstract WebView Build();
    
    protected WebViewBuilder() { }

    protected WebViewSettings Data { get; } = new();
}

// TODO File Drag and Drop (Windows)
