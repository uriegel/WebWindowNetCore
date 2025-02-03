#if Linux
using CsTools.Extensions;
using GtkDotNet;
using GtkDotNet.SafeHandles;

namespace TestAppNative.Linux;

public static class WebView
{
    public static BuilderHandle WithBuilder()
    {
        return Builder
                .FromDotNetResource("ui")
                .SideEffect(b => b.GetObject<ButtonHandle>("devtools", b => b
                    .OnClicked(() => Javascript.Run("WebView.showDevTools()"))));
    }
}
#endif