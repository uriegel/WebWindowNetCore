#if Linux
using System.Text;
using System.Text.Json;
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
            .Child(GetWebKit())
            .SideEffectIf(builder != null, window => builder!(app, window))
            .SideEffectIf(canClose != null, w => w.OnClose(_ => canClose?.Invoke() == false))
            .Show()
            .GetChild()
            .GrabFocus();

    WebViewHandle GetWebKit()
        => WebKit
                .New()
                .Ref(webViewRef)
                .SideEffect(w => w.Visible(false))
                .SideEffectIf(devTools, w => w.GetSettings().EnableDeveloperExtras = true)
                .SideEffectIf(defaultContextMenuDisabled, w => w.DisableContextMenu())
                .SideEffectIf(backgroundColor != null, w => w.BackgroundColor(backgroundColor!.Value))
                //.SideEffect(EnableResourceScheme)
                .SideEffect(Javascript.Initialize)
                .SideEffect(w => w.OnLoadChanged(OnLoad))
                .LoadUri(GetUrl());

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

    void OnLoad(WebViewHandle webView, WebViewLoad load)
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

    void OnResRequest(WebkitUriSchemeRequestHandle request)
    {
        try
        {
            var uri = "/" + request.GetUri()[6..].SubstringAfter('/').SubstringUntil('?');
            uri = uri != "/" ? uri : "/index.html";
            var res = Resources.Get(uri);
            if (res != null)
            {
                var bytes = new byte[res.Length];
                res.Read(bytes, 0, bytes.Length);
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



    Unit ShowDevTools(WebkitUriSchemeRequestHandle request)
    {
        var inspector = webViewRef.Ref.GetInspector();
        inspector.Show();
        webViewRef.Ref.GrabFocus();
        DetachInspector();
        SendOk(request);
        return Unit.Value;

        async void DetachInspector()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(600));
            inspector.Detach();
        }
    }

    Unit StartDragFiles(WebkitUriSchemeRequestHandle request)
    {
        var files = JsonSerializer.Deserialize<DragFiles>(request.GetHttpBody(), Json.Defaults);
        if (files != null)
        {
            var device = webViewRef.Ref.GetDisplay().GetDefaultSeat().GetDevice();
            using var provider = ContentProvider.NewFileUris(files.Files);
            var surface = webViewRef.Ref.GetNative().GetSurface();
            var drag = surface.DragBegin(device, provider, DragAction.Copy | DragAction.Move, 0.0, 0.0);
            drag.DragAndDropFinished(OnFinished);
            drag.DragAndDropCancelled(_ => OnFinished(false));
            SendOk(request);
            return Unit.Value;

            void OnFinished(bool success)
            {
                webViewRef.Ref.RunJavascript($"WebView.startDragFilesBack({(success ? "true" : "false")})");
                drag.Dispose();
            }
        }
        SendOk(request);
        return Unit.Value;
    }

    static Unit SendOk(WebkitUriSchemeRequestHandle request)
        => SendResponse(request, 200, "OK", "OK");

    static Unit SendNotFound(WebkitUriSchemeRequestHandle request)
        => SendResponse(request, 404, "Not Found", "I can't find what you're looking for!");

    static Unit SendResponse(WebkitUriSchemeRequestHandle request, int code, string status, string text)
    {
        using var bytes = GBytes.New(Encoding.UTF8.GetBytes(text));
        using var stream = MemoryInputStream.New(bytes);
        using var response = WebKitUriSchemeResponse.New(stream, text.Length);
        using var respondHeaders = SoupMessageHeaders.New(SoupMessageHeaderType.Response);
        respondHeaders.Set([new("Access-Control-Allow-Origin", "*")]);
        response
            .HttpHeaders(respondHeaders)
            .Status(code, status);
        request.Finish(response);
        return Unit.Value;
    }

    readonly ObjectRef<WebViewHandle> webViewRef = new();
}

record DragFiles(string[] Files);

#endif