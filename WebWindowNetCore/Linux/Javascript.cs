#if Linux
using GtkDotNet;
using GtkDotNet.SafeHandles;

namespace WebWindowNetCore.Linux;

/// <summary>
/// Run Script in WebView
/// </summary>
public static class Javascript
{
    /// <summary>
    /// /// Run Script in WebView
    /// </summary>
    /// <param name="script">Script, which should be run in WebView</param>
    public static void Run(string script)
        => webView?.RunJavascript(script);

    internal static void Initialize(WebViewHandle webView) => Javascript.webView = webView;
    static WebViewHandle? webView;
}
#endif
