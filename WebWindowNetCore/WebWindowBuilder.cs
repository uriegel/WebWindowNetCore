using System.Diagnostics;
using CsTools.Extensions;

namespace WebWindowNetCore;

// TODO rename WebView -> WebWindow
// TODO Windows version Hello World

public abstract class WebWindowBuilder
{
    /// <summary>
    /// The AppId is necessary for a webview app on Linux, it is the AppId for a GtkApplication. 
    /// It is a reverse domain name, like "de.uriegel.webapp"
    /// </summary>
    /// <param name="appId">The AppId</param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder AppId(string appId)
        => this.SideEffect(w => w.appId = appId);

    /// <summary>
    /// The window title is set by this method.
    /// </summary>
    /// <param name="title">Window title</param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder Title(string title)
        => this.SideEffect(w => w.title = title);

    /// <summary>
    /// With the help of this property you can initialize the size of the window with custom values.
    /// In combination with "SaveBounds()" this is the initial width and heigth of the window at first start,
    /// otherwise the window is always starting with these values.
    /// </summary>
    /// <param name="width">The initial width of the window</param>
    /// <param name="height">The initial height of the window</param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder InitialBounds(int width, int height)
    {
        this.width = width;
        this.height = height;
        return this;
    }

    /// <summary>
    /// When you call "SaveBounds", then windows location and width and height and normal/maximized state is saved on close. 
    /// After restarting the app the webview is displayed at these settings again.
    /// The "AppId" is used to create a path, where these settings are saved.
    /// </summary>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder SaveBounds()
        => this.SideEffect(w => w.saveBounds = true);

    /// <summary>
    ///Enable diagnostics logging in Console. Works only for Linux
    /// </summary>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder WithDiagnostics()
        => this.SideEffect(w => w.withDiagnostics = true);

    /// <summary>
    /// When called the web view's default context menu is not being displayed when you right click the mouse.
    /// </summary>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder DefaultContextMenuDisabled()
        => this.SideEffect(w => defaultContextMenuDisabled = true);

    /// <summary>
    /// This url is set to the webview only when a debugger is attached.  
    /// It is used for React, Vue,... which have their
    /// own web server at debug time, like http://localhost:3000 . If set, it has precedence over 
    /// "Url"
    /// </summary>
    /// <param name="url">The url for a web app being debugged</param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder DebugUrl(string url)
        => this.SideEffect(w => w.debugUrl = url);

    /// <summary>
    /// Here you set the url of the web view. You can use "http(s)://" scheme, "file://" scheme, and custom resource scheme "res://". This value is 
    /// not used, when you set "DebugUrl" and a debugger is attached
    /// </summary>
    /// <param name="url">The webview's url</param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder Url(string url)
        => this.SideEffect(w => w.url = url);

    /// <summary>
    /// Sets the query string to the final webroot's url
    /// </summary>
    /// <param name="queryString"></param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder QueryString(string queryString)
        => this.SideEffect(w => w.queryString = queryString);

    /// <summary>
    /// Here you can set a callback function which is called when the window is about to close. 
    /// In the callback you can prevent the close request by returning false.
    /// </summary>
    /// <param name="canCloseFunc">Callback funciton called when the window should be closed. Return "true" to close the window, "false" to prevent</param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder CanClose(Func<bool> canClose)
        => this.SideEffect(w => w.canClose = canClose);

    public abstract WebWindow Build();

    internal string GetUrl() => $"{(Debugger.IsAttached ? debugUrl ?? GetUrlOrResUrl() : GetUrlOrResUrl()) ?? "about:blank"}{queryString}";

    string? GetUrlOrResUrl() => fromResource ? "res://webwindownetcore" : url;

    internal string appId = "de.uriegel.webwindownetcore";
    internal string title = "";
    internal bool withDiagnostics;
    internal int width;
    internal int height;
    internal bool saveBounds;
    internal bool defaultContextMenuDisabled;
    internal bool fromResource;
    internal string? debugUrl;
    internal string? url;
    internal string? queryString;
    internal Func<bool>? canClose;
    internal bool useAdwaita;
}
