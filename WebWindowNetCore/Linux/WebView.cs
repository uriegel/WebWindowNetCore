using System.Net.Mime;
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
                    .SideEffectIf(devTools, w => w.GetSettings().EnableDeveloperExtras = true)
                    .SideEffectIf(backgroundColor != null, w => w.BackgroundColor(backgroundColor!.Value))
                    .SideEffectIf(GetUrl().StartsWith("res://"), EnableResourceScheme)
                    .LoadUri(GetUrl())
            )
            .Show();

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

    void OnResRequest(WebkitUriSchemeRequestHandle request)
    {
        var uri = request.GetUri()[6..];
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

    readonly ObjectRef<WebViewHandle> webViewRef = new();
}
