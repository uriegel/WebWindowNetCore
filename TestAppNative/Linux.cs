#if Linux

using GtkDotNet;
using GtkDotNet.SafeHandles;

static class Titlebar 
{
    public static WidgetHandle Create(ApplicationHandle _, WindowHandle __, ObjectRef<WebViewHandle> webview)
        => HeaderBar
            .New()
            .PackEnd(
                ToggleButton
                    .New()
                    .IconName("open-menu-symbolic")
                    .OnClicked(() => webview.Ref.GrabFocus()));
}
    
#endif
