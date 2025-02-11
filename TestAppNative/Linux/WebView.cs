#if Linux
using GtkDotNet;
using GtkDotNet.SafeHandles;

namespace TestAppNative.Linux;

public static class WebView
{
    public static void WithHeaderbar(ApplicationHandle _, WindowHandle win)
    {
        using var builder = Builder.FromDotNetResource("headerbar");
        builder.GetButton("devtools").OnClicked(() => WebWindowNetCore.WebView.RunJavascript("WebView.showDevTools()"));
        win.Titlebar(builder.GetWidget("headerBar"));
    }
}
#endif