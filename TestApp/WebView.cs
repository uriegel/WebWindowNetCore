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

                saveBounds = builder?.Data.SaveBounds == true;

                var window = new Window();
                var webView = new GtkDotNet.WebView();
                window.Add(webView);
                webView.Settings.EnableDeveloperExtras = true;
                if (builder!.Data.Url != null)
                    webView.LoadUri(builder!.Data.Url);
                if (builder?.Data.DevTools == true)
                    webView.Settings.EnableDeveloperExtras = true;
                app.AddWindow(window);
                window.SetTitle(builder?.Data.Title);
                window.SetSizeRequest(200, 200);
                window.SetDefaultSize(builder!.Data.Width, builder!.Data.Height);
                if (!saveBounds)
                    window.ShowAll();
                else
                {
                    var w = builder!.Data.Width;
                    var h = builder!.Data.Height;
                    webView.LoadChanged += (s, e) =>
                    {
                        if (e.LoadEvent == WebKitLoadEvent.WEBKIT_LOAD_COMMITTED)
                            webView.RunJavascript($"alert('show({w}, {h})')");

                        //             webView.RunJavascript(
                        //             """ 
                        //                 const button = document.getElementById('button')
                        //                 const devTools = document.getElementById('devTools')
                        //                 button.onclick = () => alert(`Das is es`)
                        //                 devTools.onclick = () => alert(`devtools`)
                        //             """);
                    };

                    window.Delete += (s, e) =>
                    {
                        webView.RunJavascript($"alert('show({w}, {h})')");
                    };
                }

                webView.ScriptDialog += (s, e) =>
                {
                    if (e.Message.StartsWith("show")) {
                        Console.WriteLine(e.Message);
                        window.Resize(1000, 500);                            
                        window.ShowAll();
                    }
                };

                builder = null;
            });

    internal WebView(WebViewBuilder builder)
        => this.builder = builder;

    WebViewBuilder? builder;

    bool saveBounds;
}

