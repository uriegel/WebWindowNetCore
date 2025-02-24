#if Linux
using GtkDotNet;
using GtkDotNet.SafeHandles;
using GtkDotNet.SubClassing;
using WebWindowNetCore;

namespace TestAppNative.Linux;

public static class Window
{
    public static void Register(ApplicationHandle app, WebView webView, string resourceTemplate)
        => app.SubClass(new CustomWindowClass(webView, resourceTemplate));
    
    class CustomWindowClass(WebView webView, string resourceTemplate)
        : SubClass<ApplicationWindowHandle>(GTypeEnum.ApplicationWindow, "CustomWindow", p => new CustomWindow(p, webView))
    {
        protected override void ClassInit(nint cls, nint _)
        {
            var webkitType = GType.Get(GTypeEnum.WebKitWebView);
            GType.Ensure(webkitType);
            var type = "WebKitWebView".TypeFromName();
            base.ClassInit(cls, _);
            InitTemplateFromResource(cls, resourceTemplate);
        }
    }

    class CustomWindow(nint obj, WebView webView) : SubClassInst<ApplicationWindowHandle>(obj)
    {
        protected override async void OnCreate()
        {
            Handle.InitTemplate();
            Handle
                .GetTemplateChild<ButtonHandle, ApplicationWindowHandle>("devtools")
                ?.OnClicked(webView.ShowDevTools);

            // TODO too early for actions
            await Task.Delay(10);

            Handle.AddActions(
                [
                    new("quit", Handle.CloseWindow, "<Ctrl>Q"),
                    new("devtools", webView.ShowDevTools, "<Ctrl><Shift>I"),
                ]);
         }
        protected override void OnFinalize() => Console.WriteLine("Window finalized");
        protected override ApplicationWindowHandle CreateHandle(nint obj) => new(obj);
    }
}
#endif