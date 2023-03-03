using GtkDotNet;

namespace WebWindow;

public static class WebViewApp
{
    public static void Run()
    {
        var app = new Application("de.uriegel.WebWindow");
        var ret = app.Run(() =>
        {
            var window = new Window();
            window.SetTitle("Web View");
            app.AddWindow(window);
            window.ShowAll();
        });
    }
}
