namespace WebWindowNetCore;
using GtkDotNet;
public static class Program
{
    public static void Execute()
    {
        var app = new Application("de.uriegel.test");
        var ret = app.Run(() => 
        {
            app.EnableSynchronizationContext();
            var webView = new WebView();
            var window = new Window();
            window.Add(webView);
            webView.LoadUri("https://www.microsoft.com");
            webView.Settings.EnableDeveloperExtras = true;
            app.AddWindow(window);
            window.SetTitle("Web View 😎😎👌");
            window.SetDefaultSize(800, 600);
            window.ShowAll();
        });
    }
}
