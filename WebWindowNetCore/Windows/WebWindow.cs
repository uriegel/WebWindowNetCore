#if Windows

using Microsoft.Web.WebView2.WinForms;

namespace WebWindowNetCore.Windows;

public class WebWindow : WebWindowNetCore.WebWindow
{
    public override int Run()
    {
        Application.Run(Window);
        return 0;
    }
    
    public WebViewForm Window { get; }
    public WebView2 WebView { get; }

    internal WebWindow(WebViewForm window, WebView2 webView)
    {
        Window = window;
        WebView = webView;
    }
}

#endif