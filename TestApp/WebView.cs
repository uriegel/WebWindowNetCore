using GtkDotNet;

public class WebView : WebWindowNetCore.WebView
{
    public static WebViewBuilder Create()
        => new WebViewBuilder();

    public override int Run(string gtkId = "de.uriegel.WebViewNetCore")
        => new Application(gtkId)
            .Run(app =>
            {
                app.EnableSynchronizationContext();

                var window = new Window();
                var webView = new GtkDotNet.WebView();
                window.Add(webView);
                webView.Settings.EnableDeveloperExtras = true;
                if (builder!.Data.UrlString != null)
                    webView.LoadUri(builder!.Data.UrlString);
                if (builder?.Data.DevTools == true)
                    webView.Settings.EnableDeveloperExtras = true;
                app.AddWindow(window);
                window.SetTitle(builder?.Data.TitleString);
                window.SetSizeRequest(builder!.Data.Width, builder!.Data.Height);
                window.ShowAll();


                builder = null;
            });

    internal WebView(WebViewBuilder builder)
        => this.builder = builder;

    WebViewBuilder? builder;
}