using System.Diagnostics;
using CsTools.Extensions;
using Microsoft.Web.WebView2.WinForms;
#if Linux
using System.Drawing;
using Gtk4DotNet;
#endif

namespace WebWindowNetCore;

public abstract class WebView
{
#if Windows
    public static WebView Create() => new Windows.WebView();
#elif Linux
    public static WebView Create() => new Linux.WebView();

    /// <summary>
    /// Creates a window from a GtkBuilder template which is contained in .Net resource.
    /// The ApplicationWindow in the template has to have the id "window". The template has to contain a webkit webView with the id "webview". 
    /// </summary>
    /// <param name="template">Name of the .NET resource containing the Gtk4 template</param>
    /// <param name="onActivate">Is called on activation of the Gtk4 app. In this callback the builder ui .</param>
    /// <param name="useAdwaita">If true, an Adwaita Application is created instead of a GtkApplication</param>
    /// <returns></returns>
    public WebView FromResourceTemplate(string template, Func<Application, WindowBuilder, ApplicationWindow> onActivate, bool useAdwaita = false)
    {
        resourceTemplate = template;
        this.useAdwaita = useAdwaita;
        this.onActivate = onActivate;
        return this;
    }

#endif

    public abstract bool IsMaximized { get; }

    /// <summary>
    /// The AppId is necessary for a webview app on Linux, it is the AppId for a GtkApplication. 
    /// It is a reverse domain name, like "de.uriegel.webapp"
    /// </summary>
    /// <param name="appId">The AppId</param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView AppId(string appId)
        => this.SideEffect(w => w.appId = appId);

    /// <summary>
    /// With the help of this property you can initialize the size of the window with custom values.
    /// In combination with "SaveBounds()" this is the initial width and heigth of the window at first start,
    /// otherwise the window is always starting with these values.
    /// </summary>
    /// <param name="width">The initial width of the window</param>
    /// <param name="height">The initial height of the window</param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView InitialBounds(int width, int height)
    {
        this.width = width;
        this.height = height;
        return this;
    }

    /// <summary>
    /// The window title is set by this method.
    /// </summary>
    /// <param name="title">Window title</param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView Title(string title)
        => this.SideEffect(w => w.title = title);

    /// <summary>
    /// Setting the background color of the web view. Normally the html page has its own background color, 
    /// but when starting and before the html page is loaded, this property is active and this color is shown. 
    /// To prevent flickering when starting the app, adapt the BackgroundColor to the http page's value.
    /// </summary>
    /// <param name="color">Background color</param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView BackgroundColor(Color color)
        => this.SideEffect(w => w.backgroundColor = color);

    /// <summary>
    /// Here you set the url of the web view. You can use "http(s)://" scheme, "file://" scheme, and custom resource scheme "res://". This value is 
    /// not used, when you set "DebugUrl" and a debugger is attached
    /// </summary>
    /// <param name="url">The webview's url</param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView Url(string url)
        => this.SideEffect(w => w.url = url);

    /// <summary>
    /// This url is set to the webview only when a debugger is attached.  
    /// It is used for React, Vue,... which have their
    /// own web server at debug time, like http://localhost:3000 . If set, it has precedence over 
    /// "Url"
    /// </summary>
    /// <param name="url">The url for a web app being debugged</param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView DebugUrl(string url)
        => this.SideEffect(w => w.debugUrl = url);

    public WebView FromResource()
        => this.SideEffect(w => w.fromResource = true);

    /// <summary>
    /// When you call "SaveBounds", then windows location and width and height and normal/maximized state is saved on close. 
    /// After restarting the app the webview is displayed at these settings again.
    /// The "AppId" is used to create a path, where these settings are saved.
    /// </summary>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView SaveBounds()
        => this.SideEffect(w => w.saveBounds = true);

    /// <summary>
    ///Enumerable diagnostics logging in Console. Works only for Linux
    /// </summary>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView WithDiagnostics()
        => this.SideEffect(w => w.withDiagnostics = true);

    /// <summary>
    /// Used to enable (not to show) the developer tools. If not called, it is not possible to open these tools.
    /// The developer tools can be shown by default context menu or by calling the javascript method WebView.showDevtools()
    /// </summary>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView DevTools()
        => this.SideEffect(w => devTools = true);

    public WebView OnStateChange(Action onStateChanged)
        => this.SideEffect(w => w.onStateChanged = onStateChanged);

    /// <summary>
    /// When called the web view's default context menu is not being displayed when you right click the mouse.
    /// </summary>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView DefaultContextMenuDisabled()
        => this.SideEffect(w => defaultContextMenuDisabled = true);

    /// <summary>
    /// Sets the query string to the final webroot's url
    /// </summary>
    /// <param name="queryString"></param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView QueryString(string queryString)
        => this.SideEffect(w => w.queryString = queryString);

    public WebView ScriptDialog(Action<string> onAlert)
        => this.SideEffect(w => w.onAlert = onAlert);

    /// <summary>
    /// Here you can set a callback function which is called when the window is about to close. 
    /// In the callback you can prevent the close request by returning false.
    /// </summary>
    /// <param name="canCloseFunc">Callback funciton called when the window should be closed. Return "true" to close the window, "false" to prevent</param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView CanClose(Func<bool> canClose)
        => this.SideEffect(w => w.canClose = canClose);

    public abstract void ShowDevTools();
    public abstract Task StartDragFiles(string[] dragFiles);
    public abstract void RunJavascript(string script);
    public abstract void Close();
    public abstract void Minimize();
    public abstract void Maximize();
    public abstract void Restore();
    public abstract void BeginInvoke(Action action);
    public abstract Task<T> BeginInvoke<T>(Func<T> func);
    public abstract void SetFocus();

#if Windows
    public WebView OnCreating(Action<Form, WebView2> onCreate)
        => this.SideEffect(w => w.onCreate = onCreate);

    /// <summary>
    /// Hides the Windows Titlebar
    /// </summary>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView WithoutNativeTitlebar()
        => this.SideEffect(w => w.withoutNativeTitlebar = true);

    /// <summary>
    /// Used to display a windows icon from C# resource. It is only working on Windows.
    /// </summary>
    /// <param name="iconName">Logical name of the resource icon</param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView ResourceIcon(string icon)
        => this.SideEffect(w => w.resourceIcon = icon);

#endif

    /// <summary>
    /// Runs the built app and displays the web view
    /// </summary>
    /// <returns>Exit code</returns>
    public abstract int Run();

    internal string GetUrl() => $"{(Debugger.IsAttached ? debugUrl ?? GetUrlOrResUrl() : GetUrlOrResUrl()) ?? "about:blank"}{queryString}";
    internal string appId = "de.uriegel.webwindownetcore";
    internal int width;
    internal int height;
    internal string title = "";
    internal Color? backgroundColor;
    internal string? url;
    internal string? debugUrl;
    internal bool saveBounds;
    internal bool withDiagnostics;
    internal bool devTools;
    internal bool defaultContextMenuDisabled;
    internal bool fromResource;
    internal string? queryString;
    internal Action<string>? onAlert;
    internal Func<bool>? canClose;
    internal Action? onStateChanged;
    
#if Windows
    internal bool withoutNativeTitlebar;
    internal string? resourceIcon;
    internal Action<Form, WebView2>? onCreate;
#endif
#if Linux
    protected string? resourceTemplate;
    protected bool useAdwaita;
    protected Func<Application, WindowBuilder, ApplicationWindow>? onActivate;
#endif
    string? GetUrlOrResUrl() => fromResource ? "res://webwindownetcore" : url;
}

// TODO WebView requests Linux
// TODO startDragFiles await till dropped or finished
// TODO dropFiles 
// TODO Linux: Enable Resource Scheme (disposing error)