#if Linux
using CsTools.Extensions;
using GtkDotNet;
using GtkDotNet.SafeHandles;

namespace TestAppNative.Linux;

public static class WebView
{
    public static BuilderHandle WithBuilder(ApplicationHandle _)
    {
        return Builder
                .FromDotNetResource("ui")
                .SideEffect(b => b.GetObject<ButtonHandle>("devtools", b => b
                    .OnClicked(() => WebWindowNetCore.WebView.RunJavascript("WebView.showDevTools()"))));
    }
}
#endif