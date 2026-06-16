using System.Diagnostics;
using System.Drawing;
using CsTools.Extensions;
#if Linux
using Gtk4DotNet;
#endif

namespace WebWindowNetCore;

// TODO WebView requests Linux
// TODO startDragFiles await till dropped or finished
// TODO dropFiles 
// TODO Linux: Enable Resource Scheme (disposing error)

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
    public WebWindowBuilder WithDiagnostics(bool logging = false)
    {
        withDiagnostics = true;
        withDiagnosticsLogging = logging;
        return this;
    }
        
    /// <summary>
    /// Used to enable (not to show) the developer tools. If not called, it is not possible to open these tools.
    /// The developer tools can be shown by default context menu or by calling the javascript method WebView.showDevtools()
    /// </summary>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder DevTools()
        => this.SideEffect(w => devTools = true);

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

    // TODO Description
    /// <summary>
    /// 
    /// </summary>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder FromResource()
        => this.SideEffect(w => w.fromResource = true);

#if Linux
    /// <summary>
    /// Creates a window from a GtkBuilder template which is contained in .Net resource.
    /// The ApplicationWindow in the template has to have the id "window". The template has to contain a webkit webView with the id "webview". 
    /// </summary>
    /// <param name="template">Name of the .NET resource containing the Gtk4 template</param>
    /// <param name="onActivate">Is called on activation of the Gtk4 app. In this callback the builder ui .</param>
    /// <param name="useAdwaita">If true, an Adwaita Application is created instead of a GtkApplication</param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder FromResourceTemplate(string template, Func<WebWindow, WindowBuilder, ApplicationWindow> onActivate, bool useAdwaita = false)
    {
        resourceTemplate = template;
        this.useAdwaita = useAdwaita;
        this.onActivate = onActivate;
        return this;
    }
#endif

    /// <summary>
    /// Setting the background color of the web view. Normally the html page has its own background color, 
    /// but when starting and before the html page is loaded, this property is active and this color is shown. 
    /// To prevent flickering when starting the app, adapt the BackgroundColor to the http page's value.
    /// </summary>
    /// <param name="color">Background color</param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder BackgroundColor(Color color)
        => this.SideEffect(w => w.backgroundColor = color);

    /// <summary>
    /// Here you can set a callback function which is called when the window is about to close. 
    /// In the callback you can prevent the close request by returning false.
    /// </summary>
    /// <param name="canCloseFunc">Callback funciton called when the window should be closed. Return "true" to close the window, "false" to prevent</param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder CanClose(Func<bool> canClose)
        => this.SideEffect(w => w.canClose = canClose);

    /// <summary>
    /// If the Window State changed, if it is maximized or restored, this callback is to be called
    /// </summary>
    /// <param name="onStateChanged"></param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder OnStateChange(Action<WebWindow> onStateChanged)
        => this.SideEffect(w => w.onStateChanged = onStateChanged);

    /// <summary>
    /// If you want your own implementation of the alert messagebox, or if you want to communicate from javascript to the WebWindow
    /// </summary>
    /// <param name="onAlert"></param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder ScriptDialog(Action<WebWindow, string> onAlert)
        => this.SideEffect(w => w.onAlert = onAlert);

#if Windows

    /// <summary>
    /// When the WebWindow is being created, this callback will be invoked
    /// </summary>
    /// <param name="onCreate"></param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder OnCreating(Action<WebWindow> onCreate)
        => this.SideEffect(w => w.onCreate = onCreate);

    /// <summary>
    /// Used to display a windows icon from C# resource. It is only working on Windows.
    /// </summary>
    /// <param name="iconName">Logical name of the resource icon</param>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder ResourceIcon(string icon)
        => this.SideEffect(w => w.resourceIcon = icon);

    /// <summary>
    /// Hides the Windows Titlebar
    /// </summary>
    /// <returns>WebWindowBuilder for chaining (Fluent Builder Syntax)</returns>
    public WebWindowBuilder WithoutNativeTitlebar()
        => this.SideEffect(w => w.withoutNativeTitlebar = true);

#endif

    /// <summary>
    /// Builds a WebWindow with previously set properties
    /// </summary>
    /// <returns></returns>
    public abstract WebWindow Build();

    internal string GetUrl() => $"{(Debugger.IsAttached ? debugUrl ?? GetUrlOrResUrl() : GetUrlOrResUrl()) ?? "about:blank"}{queryString}";

    string? GetUrlOrResUrl() => fromResource ? "res://webwindownetcore" : url;

    internal string appId = "de.uriegel.webwindownetcore";
    internal string title = "";
    internal bool withDiagnostics;
    internal bool withDiagnosticsLogging;
    internal int width;
    internal int height;
    internal bool saveBounds;
    internal bool defaultContextMenuDisabled;
    internal bool fromResource;
    internal string? debugUrl;
    internal string? url;
    internal string? queryString;
    internal Func<bool>? canClose;
    internal Color backgroundColor = Color.Transparent;
    internal Action<WebWindow>? onStateChanged;
    internal Action<WebWindow, string>? onAlert;
    internal bool devTools;
#if Linux
    internal string? resourceTemplate;
    internal bool useAdwaita;
    internal Func<WebWindow, WindowBuilder, ApplicationWindow>? onActivate;
#endif
#if Windows
    internal Action<WebWindow>? onCreate;
    internal string? resourceIcon;
    internal bool withoutNativeTitlebar;
#endif
}
