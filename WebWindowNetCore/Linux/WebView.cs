#if Linux
using CsTools.Extensions;
using Gtk4DotNet;

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

    public override async Task StartDragFiles(string[] dragFiles) { }

    public override void RunJavascript(string script) => webView?.RunJavascript(script);

    void OnActivate(Application app)
    {
        onActivate?.Invoke(app, this, resourceTemplate!);
        var window = app.CreateWindow(adwResource, resourceTemplate);
        window.Title = title;
        if (saveBounds)
            WithSaveBounds(window);
        else
            window.DefaultSize(width, height);
        webView = GetWebKit(window);
        window.Child(webView);
        if (canClose != null)
            window.OnClose(_ => canClose() == false);
        window.Show();
        webView.GrabFocus();
    }

    Gtk4DotNet.WebView GetWebKit(ApplicationWindow window)
        => CreateWebKit(window)
            .SideEffect(w => w.Visible(false))
            .SideEffectIf(devTools, w => w.GetSettings().EnableDeveloperExtras = true)
            .SideEffectIf(defaultContextMenuDisabled, w => w.DisableContextMenu())
            .SideEffectIf(backgroundColor != null, w => w.BackgroundColor(backgroundColor!.Value))
            .SideEffect(w => w.OnLoadChanged(OnLoad))
            .LoadUri(GetUrl());

    Gtk4DotNet.WebView CreateWebKit(ApplicationWindow window)
        // => webView = resourceTemplate == null
        //     ? Gtk4DotNet.WebView.New()
        //     : window.GetTemplateChild<WebView, ApplicationWindow>("webview") ?? Gtk4DotNet.WebView.New();

        => Gtk4DotNet.WebView.New();

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

    Gtk4DotNet.WebView? webView;
}

static class WebViewExtensions
{
    public static ApplicationWindow CreateWindow(this Application app, bool adw, string? resourceTemplate)
        => app.NewWindow();
        // => resourceTemplate == null
        //     ? app.NewWindow()
        //     : adw
        //     ? app.CreateWindow()
        //     : app.CustomWindow("CustomWindow");
}

#endif


