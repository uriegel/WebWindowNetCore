using System.Text;
using CsTools;
using CsTools.Extensions;
using GtkDotNet;
using GtkDotNet.SafeHandles;

namespace WebWindowNetCore.Linux;

public class WebView() : WebWindowNetCore.WebView
{
    public override int Run() =>
        Application.NewAdwaita(appId)
            .OnActivate(OnActivate)
            .Run(0, 0);

    void OnActivate(ApplicationHandle app)
        => app
            .NewWindow()
            .Title(title)
            .SideEffectChoose(saveBounds, WithSaveBounds, w => w.DefaultSize(width, height))
            .Child(WebKit
                    .New()
                    .Ref(webViewRef)
                    .SideEffect(w => w.Visible(false))
                    .SideEffectIf(devTools, w => w.GetSettings().EnableDeveloperExtras = true)
                    .SideEffectIf(defaultContextMenuDisabled, w => w.DisableContextMenu())
                    .SideEffectIf(backgroundColor != null, w => w.BackgroundColor(backgroundColor!.Value))
                    .SideEffectIf(GetUrl().StartsWith("res://"), EnableResourceScheme)
                    .SideEffectIf(request != null, EnableRequests)
                    .SideEffect(EnableRequestScheme)
                    .SideEffect(Javascript.Initialize)
                    .SideEffect(w => w.OnLoadChanged(OnLoad))
                    .LoadUri(GetUrl())
            )
            .SideEffectIf(canClose != null, w => w.OnClose(_ => canClose?.Invoke() == false))
            .Show()
            .GrabFocus();

    void WithSaveBounds(WindowHandle window)
        => Bounds
            .Retrieve(appId)
            .SideEffect(b => window.DefaultSize(b.Width ?? width, b.Height ?? height))
            .SideEffectIf(b => b.IsMaximized, _ => window.Maximize())
            .SideEffect(_ => window.OnClose(SaveBounds));

    bool SaveBounds(WindowHandle window)
        => false.SideEffect(_ =>
                Bounds
                    .Save(appId, Bounds.Retrieve(appId) with
                    {
                        Width = window.GetWidth(),
                        Height = window.GetHeight(),
                        IsMaximized = window.IsMaximized()
                    }));

    void EnableResourceScheme(WebViewHandle webView)
        => WebKitWebContext.GetDefault().RegisterUriScheme("res", OnResRequest);

    void EnableRequestScheme(WebViewHandle webView)
        => WebKitWebContext.GetDefault().RegisterUriScheme("req", OnReqRequest);

    void OnLoad(WebViewHandle webView, WebViewLoad load)
    {
        if (load == WebViewLoad.Committed)
        {
            webView.RunJavascript(WebWindowNetCore.ScriptInjection.Get());
            SetVisible();
            
            async void SetVisible()
            {
                await Task.Delay(TimeSpan.FromMilliseconds(20));
                webView.Visible(true);
            }
        }
    }

    void OnResRequest(WebkitUriSchemeRequestHandle request)
    {
        var uri = request.GetUri()[6..].SubstringUntil('?');
        var res = Resources.Get(uri);
        if (res != null) 
        {
            var bytes = new byte[res.Length];
            res.Read(bytes, 0, bytes.Length);
            using var gbytes = GBytes.New(bytes);
            using var gstream = MemoryInputStream.New(gbytes);
            request.Finish(gstream, bytes.Length, uri?.GetFileExtension()?.ToMimeType() ?? "text/html");
        }
    }

    void OnReqRequest(WebkitUriSchemeRequestHandle request)
    {
        var _ = request.GetUri()[6..] switch
        {
            "showDevTools" => ShowDevTools(request),
            _ => SendOk(request)
        };
    }

    Unit ShowDevTools(WebkitUriSchemeRequestHandle request)
    {
        var inspector = webViewRef.Ref.GetInspector();
        inspector.Show();
        webViewRef.Ref.GrabFocus();
        DetachInspector();
        SendOk(request);

        async void DetachInspector()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(600));
            inspector.Detach();
        }
        return Unit.Value;
    }

    Unit SendOk(WebkitUriSchemeRequestHandle request)
    {
        var ok = "OK";
        using var bytes = GBytes.New(Encoding.UTF8.GetBytes(ok));
        using var stream = MemoryInputStream.New(bytes);
        request.Finish(stream, ok.Length, "text/html");
        return Unit.Value;
    }

    void EnableRequests(WebViewHandle webView)
        => webView.OnAlert(OnRequest);

    void OnRequest(WebViewHandle webView, string? msg)        
    {
        if (request != null && msg?.StartsWith("request") == true)
        {
            var req = Request.Create(webView, msg);
            request(req);
        }
    }

    readonly ObjectRef<WebViewHandle> webViewRef = new();
}
