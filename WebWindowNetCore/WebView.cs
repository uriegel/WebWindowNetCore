using CsTools.Extensions;

namespace WebWindowNetCore;

public abstract class WebView
{
#if Linux
    public static WebView Create() => new Linux.WebView();
#endif    

    /// <summary>
    /// The AppId is necessary for a webview app on Linux, it is the AppId for a GtkApplication. 
    /// It is a reverse domain name, like "de.uriegel.webapp"
    /// </summary>
    /// <param name="appId">The AppId</param>
    /// <returns>WebView for chaining (Fluent Builder Syntax)</returns>
    public WebView AppId(string appId)
        => this.SideEffect(w => w.appId = appId);



    /// <summary>
    /// Runs the built app and displays the web view
    /// </summary>
    /// <returns>Exit code</returns>
    public abstract int Run();

    string appId = "de.uriegel.webwindownetcore";
}
