#if Linux

using System.Text;
using CsTools.Extensions;
using Gtk4DotNet;

namespace WebWindowNetCore.Linux;

public class WebWindow : WebWindowNetCore.WebWindow
{
    public Application Application { get; }
    public ApplicationWindow Window { get; internal set; } = null!;
    public Gtk4DotNet.WebView WebView { get; internal set; } = null!;
    public override bool IsMaximized { get => Window.IsMaximized == true; }

    public override int Run() => Application.Run(0, 0);

    public override async void ShowDevTools()
    {
        try
        {
            await Gtk.InvokeAsync(() =>
            {
                var inspector = WebView.GetInspector();
                inspector.Show();
                WebView.GrabFocus();
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
    public override async Task StartDragFiles(string[] dragFiles) {}
    public override void RunJavascript(string script) => WebView.RunJavascript(script);
    public override void Close() => Window.CloseWindow();
    public override void Minimize() { }
    public override void Maximize() => Window.IsMaximized = true;
    public override void Restore() => Window.IsMaximized = false;
    public override void BeginInvoke(Action action) => Gtk.InvokeAsync(action);
    public override Task<T> InvokeAsync<T>(Func<T> func) => Gtk.InvokeAsync(func);
    public override void SetFocus() => WebView.GrabFocus();


    internal static void WithSaveBounds(Window window, string appId, int width, int height)
        => Bounds
            .Retrieve(appId)
            .SideEffect(b => window.DefaultSize(b.Width ?? width, b.Height ?? height))
            .SideEffectIf(b => b.IsMaximized, _ => window.IsMaximized = true)
            .SideEffect(_ => window.OnClose(w => SaveBounds(w, appId)));

    static bool SaveBounds(Window window, string appId)
        => false.SideEffect(_ =>
                Bounds
                    .Save(appId, Bounds.Retrieve(appId) with
                    {
                        Width = window.Width,
                        Height = window.Height,
                        IsMaximized = window.IsMaximized
                    }));
    internal static void OnResRequest(WebkitUriSchemeRequest request)
    {
        try
        {
            var uri = request.GetUri()[6..].SubstringAfter('/').SubstringUntil('?');
            uri = uri.Length > 0 ? uri : "index.html";
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

    internal WebWindow(Application application) => Application = application;
}

#endif