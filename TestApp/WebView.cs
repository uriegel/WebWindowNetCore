using CsTools.Extensions;
using GtkDotNet;
using LinqTools;
using WebWindowNetCore.Data;

public class WebView : WebWindowNetCore.WebView
{
    public static WebViewBuilder Create()
        => new WebViewBuilder();

    public override int Run(string gtkId = "de.uriegel.WebViewNetCore")
        => new Application(gtkId)
            .Run(app =>
            {
            app.EnableSynchronizationContext();

            GtkDotNet.Timer? timer = null;
            saveBounds = settings?.SaveBounds == true;

            var window = new Window();
            var webView = new GtkDotNet.WebView();
            window.Add(webView);
            webView.Settings.EnableDeveloperExtras = true;
            if (settings?.Url != null)
                webView.LoadUri(settings?.Url);
            if (settings?.DevTools == true)
                webView.Settings.EnableDeveloperExtras = true;
            app.AddWindow(window);
            window.SetTitle(settings?.Title);
            window.SetSizeRequest(200, 200);
            window.SetDefaultSize(settings!.Width, settings!.Height);
            if (!saveBounds)
                window.ShowAll();
            else
            {
                var w = settings?.Width;
                var h = settings?.Height;
                webView.LoadChanged += (s, e) =>
                {
                    if (e.LoadEvent == WebKitLoadEvent.WEBKIT_LOAD_COMMITTED)
                        webView.RunJavascript(
                        """ 
                            const devTools = document.getElementById('devTools')
                            devTools.onclick = () => alert(`devtools`)

                            const bounds = JSON.parse(localStorage.getItem('window-bounds') || '{}')
                            const isMaximized = localStorage.getItem('isMaximized')
                            if (bounds.width && bounds.height)
                                alert(`show(${bounds.width}, ${bounds.height}, isMaximized)`)
                            else
                                alert('initialShow')
                        """);
                };

                window.Configure += (s, e) =>
                {
                    timer?.Dispose();
                    timer = new(() => 
                    {
                        if (window.IsMaximized())
                            webView.RunJavascript(
                                $$"""
                                    localStorage.setItem('window-bounds', JSON.stringify({width: {{e.Width}}, height: {{e.Height}}}))
                                    localStorage.setItem('isMaximized', false)
                                """);
                        else
                            webView.RunJavascript($"localStorage.setItem('isMaximized', true)");
                    }, TimeSpan.FromMilliseconds(400), Timeout.InfiniteTimeSpan);
                };
            }

            webView.ScriptDialog += (s, e) =>
            {
                if (e.Message.StartsWith("show"))
                {
                    var width = e
                        .Message
                        .StringBetween('(', ',')
                        .ParseInt()
                        .GetOrDefault(200);
                    var height = e
                        .Message
                        .StringBetween(',', ',')
                        .ParseInt()
                        .GetOrDefault(200);
                    var isMaximized = e
                        .Message
                        .StringBetween(',', ')')
                        .GetOrDefault("false");
                    window.Resize(width, height);                            
                    if (isMaximized == "true")
                        window.Maximize();
                    window.ShowAll();
                } else if (e.Message == "devtools")
                    webView.Inspector.Show();
                else if (e.Message == "initialShow")
                    window.ShowAll();   
            };

            settings = null;
        });

    internal WebView(WebViewBuilder builder)
        => settings = builder.Data;

    WebViewSettings? settings;

    bool saveBounds;
}

