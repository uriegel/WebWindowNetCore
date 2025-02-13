#if Linux
using GtkDotNet;
using GtkDotNet.SafeHandles;

namespace TestAppNative.Linux;

public static class WebView
{
    public static void WithHeaderbar(WebWindowNetCore.WebView webView, ApplicationHandle _, WindowHandle win)
    {
        using var builder = Builder.FromDotNetResource("headerbar");
        builder.GetButton("devtools").OnClicked(webView.ShowDevTools);
        win.Titlebar(builder.GetWidget("headerBar"));
    }
}
#endif