#if Linux
using CsTools.Extensions;
using GtkDotNet;
using GtkDotNet.SafeHandles;

namespace TestAppNative.Linux;

public static class WebView
{
    public static WidgetHandle WithHeaderbar(ApplicationHandle _, WindowHandle __)
        => Builder
                .FromDotNetResource("headerbar")
                .SideEffect(b => b.GetButton("devtools").OnClicked(() => WebWindowNetCore.WebView.RunJavascript("WebView.showDevTools()"))                )
                .GetWidget("headerBar");
}
#endif