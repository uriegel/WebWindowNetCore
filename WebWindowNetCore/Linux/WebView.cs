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

    public override async Task StartDragFiles(string[] dragFiles)
    {
        try
        {
            await Gtk.Dispatch(() =>
            {
                var device = webView!.GetDisplay().GetDefaultSeat().GetDevice();
                using var provider = ContentProvider.NewFileUris(dragFiles);
                var surface = webView!.GetNative().GetSurface();
                var drag = surface.DragBegin(device, provider, DragAction.Copy | DragAction.Move, 0.0, 0.0);
                drag.DragAndDropFinished(OnFinished);
                drag.DragAndDropCancelled(_ => OnFinished(false));

                void OnFinished(bool success)
                {
                    // TODO TaskCompletionSource: return StartDragFiles
                    webView!.RunJavascript($"WebView.startDragFilesBack({(success ? "true" : "false")})");
                    drag.Dispose();
                }
            });
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Could not show devtools: {e}");
        }
    }

    public override void RunJavascript(string script)
        => webView!.RunJavascript(script);

    void OnActivate(ApplicationHandle app)
        => app
            .NewWindow()
            .Title(title)
            .SideEffectChoose(saveBounds, WithSaveBounds, w => w.DefaultSize(width, height))
            .Child(GetWebKit())
            .SideEffectIf(builder != null, window => builder!(this, app, window))
            .SideEffectIf(canClose != null, w => w.OnClose(_ => canClose?.Invoke() == false))
            .Show()
            .GetChild()
            .GrabFocus();

    WebViewHandle GetWebKit()
        => CreateWebKit()
            .SideEffect(w => w.Visible(false))
            .SideEffectIf(devTools, w => w.GetSettings().EnableDeveloperExtras = true)
            .SideEffectIf(defaultContextMenuDisabled, w => w.DisableContextMenu())
            .SideEffectIf(backgroundColor != null, w => w.BackgroundColor(backgroundColor!.Value))
            .SideEffectIf(fromResource, EnableResourceScheme)
            .SideEffect(w => w.OnLoadChanged(OnLoad))
            .LoadUri(GetUrl());

    WebViewHandle CreateWebKit()
        => webView = WebKit.New();

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

    WebViewHandle? webView;
}

#endif