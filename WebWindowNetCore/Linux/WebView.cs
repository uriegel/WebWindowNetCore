#if Linux
using System.Text;
using CsTools.Extensions;
using Gtk4DotNet;

namespace WebWindowNetCore.Linux;

public class WebView() : WebWindowNetCore.WebView
{
    public override int Run()
    {
        var app = useAdwaita ? Application.NewAdwaita(appId) : Application.New(appId);
        if (withDiagnostics)
            app.WithDiagnostics();
        return app.OnActivate(OnActivate)
           .Run(0, 0);
    }

    public override async void ShowDevTools()
    {
        try
        {
            await Gtk.Dispatch(() =>
            {
                var inspector = webView!.GetInspector();
                inspector.Show();
                webView!.GrabFocus();
                DetachInspector();

                async void DetachInspector()
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(600));
                    inspector.Detach();
                }
            });
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Could not show devtools: {e}");
        }
    }

    public override async Task StartDragFiles(string[] dragFiles) { }

    public override void RunJavascript(string script) => webView?.RunJavascript(script);

    void OnActivate(Application app)
    {
        var window = resourceTemplate != null && onActivate != null
            ? app.WithWebKit().WindowFromBuilder(resourceTemplate, "window", builder =>
            {
                var window = onActivate(app, builder);
                webView = builder.Builder.GetWidget<Gtk4DotNet.WebView>("webview");
                return window;
            })
            : app.NewWindow();
        
        window.Title = title;
        if (saveBounds)
            WithSaveBounds(window);
        else
            window.DefaultSize(width, height);
        webView ??= Gtk4DotNet.WebView.New();
        window.Child(webView);
        if (canClose != null)
            window.OnClose(_ => canClose() == false);

        webView.Visible = false;
        if (devTools)
            webView.GetSettings().EnableDeveloperExtras = true;
        if (defaultContextMenuDisabled)
            webView.DisableContextMenu();
        if (backgroundColor.HasValue)
            webView.BackgroundColor(backgroundColor.Value);
        if (fromResource)
            WebKitWebContext.GetDefault().RegisterUriScheme("res", OnResRequest);
        webView.OnLoadChanged(OnLoad);
        webView.LoadUri(GetUrl());

        window.Show();
        webView.GrabFocus();
    }

    void WithSaveBounds(Window window)
        => Bounds
            .Retrieve(appId)
            .SideEffect(b => window.DefaultSize(b.Width ?? width, b.Height ?? height))
            .SideEffectIf(b => b.IsMaximized, _ => window.IsMaximized = true)
            .SideEffect(_ => window.OnClose(SaveBounds));

    bool SaveBounds(Window window)
        => false.SideEffect(_ =>
                Bounds
                    .Save(appId, Bounds.Retrieve(appId) with
                    {
                        Width = window.Width,
                        Height = window.Height,
                        IsMaximized = window.IsMaximized
                    }));

    void OnLoad(Gtk4DotNet.WebView webView, WebViewLoad load)
    {
        if (load == WebViewLoad.Committed)
        {
            SetVisible();

            async void SetVisible()
            {
                await Task.Delay(TimeSpan.FromMilliseconds(20));
                webView.Visible(true);
            }
        }
    }

    void OnResRequest(WebkitUriSchemeRequest request)
    {
        try
        {
            var uri = "/" + request.GetUri()[6..].SubstringAfter('/').SubstringUntil('?');
            uri = uri != "/" ? uri : "/index.html";
            var res = Resources.Get(uri);
            if (res != null)
            {
                var bytes = new byte[res.Length];
                var read = res.Read(bytes, 0, bytes.Length);
                using var gbytes = GBytes.New(bytes);
                using var gstream = MemoryInputStream.New(gbytes);
                request.Finish(gstream, bytes.Length, uri?.GetFileExtension()?.ToMimeType() ?? "text/html");
            }
            else
                SendNotFound(request);
        }
        catch
        {
            SendNotFound(request);
        }
    }

    static void SendNotFound(WebkitUriSchemeRequest request)
        => SendResponse(request, 404, "Not Found", "I can't find what you're looking for!");

    static void SendResponse(WebkitUriSchemeRequest request, int code, string status, string text)
    {
        using var bytes = GBytes.New(Encoding.UTF8.GetBytes(text));
        using var stream = MemoryInputStream.New(bytes);
        using var response = WebKitUriSchemeResponse.New(stream, text.Length);
        using var respondHeaders = SoupMessageHeaders.New(SoupMessageHeaderType.Response);
        respondHeaders.Set([new("Access-Control-Allow-Origin", "*")]);
        response.HttpHeaders(respondHeaders);
        response.Status(code, status);
        request.Finish(response);
    }

    Gtk4DotNet.WebView? webView;
}

#endif


// TODO CheckDiagnostics: FromResource 1 delegate remaining
// TODO UnregisterUriScheme
// TODO Check Commander
// TODO First beta
// TODO with WebServerLight