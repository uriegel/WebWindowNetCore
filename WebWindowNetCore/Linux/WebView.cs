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
                    .SideEffect(EnableRequestScheme)
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
        switch (request.GetUri()[6..])            
        {
            case "showDevTools":
                var inspector = webViewRef.Ref.GetInspector();
                inspector.Show();
                webViewRef.Ref.GrabFocus();
                DetachInspector();

                async void DetachInspector()
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(600));
                    inspector.Detach();
                }
                break;
            default:
                // TODO
                // WebkitView::send_response(req, 404, "Not Found", html::ok());
                return;
        }
        // TODO
        // WebkitView::send_response(req, 200, "Ok", html::ok());
    }

    readonly ObjectRef<WebViewHandle> webViewRef = new();
}
