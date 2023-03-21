using WebWindowNetCore.Data;
using static ClrWinApi.Api;

namespace WebWindowNetCore;

enum Action
{
    DevTools = 1,
    Show,
}

record ScriptAction(Action Action, int? Width, int? Height, bool? IsMaximized);

public class WebView : WebWindowNetCore.Base.WebView
{
    public static WebViewBuilder Create()
        => new WebViewBuilder();

    public override int Run(string gtkId = "")
    {
        var webForm = new WebWindowForm(settings, appDataPath);
        webForm.Show();
        webForm.FormClosed += (s, e) => PostQuitMessage(0);

        while (GetMessage(out var msg, IntPtr.Zero, 0, 0) != 0)
            DispatchMessage(ref msg);
        return 0;
    }

    internal WebView(WebViewBuilder builder)
    {
        appDataPath = builder.AppDataPath;
        settings = builder.Data;
    }
        
    string appDataPath;
    WebViewSettings? settings;
}
