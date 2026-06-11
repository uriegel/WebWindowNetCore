#if Linux

using CsTools.Extensions;
using Gtk4DotNet;

namespace WebWindowNetCore.Linux;

public class WebWindowBuilder : WebWindowNetCore.WebWindowBuilder
{
    public override WebWindowNetCore.WebWindow Build()
    {
        var app = useAdwaita ? Application.NewAdwaita(appId) : Application.New(appId);
        if (withDiagnostics)
            app.WithDiagnostics();
        var tcs = new TaskCompletionSource<WebWindowNetCore.WebWindow>();
        var webWindow = new WebWindow(app);
        app.OnActivate(app => OnActivate(app, webWindow));
        return webWindow;
    }

    void OnActivate(Application app, WebWindow webWindow)
    {
        webWindow.Window = app.NewWindow();
        // window = resourceTemplate != null && onActivate != null
        //     ? app.WithWebKit().WindowFromBuilder(resourceTemplate, "window", builder =>
        //     {
        //         var window = onActivate(app, builder);
        //         webView = builder.Builder.GetWidget<Gtk4DotNet.WebView>("webview");
        //         return window;
        //     })
        //     : app.NewWindow();

        webWindow.Window.Title = title;
        if (saveBounds)
            WithSaveBounds(webWindow.Window);
        else
            webWindow.Window.DefaultSize(width, height);
        // if (onStateChanged != null)
        //     window.OnNotify("maximized", onStateChanged);
        webWindow.WebView = Gtk4DotNet.WebView.New();
        webWindow.Window.Child(webWindow.WebView);
        if (canClose != null)
            webWindow.Window.OnClose(_ => canClose() == false);

        webWindow.WebView.Visible = false;
        // if (devTools)
        //     webView.GetSettings().EnableDeveloperExtras = true;
        if (defaultContextMenuDisabled)
            webWindow.WebView.DisableContextMenu();
        webWindow.WebView.BackgroundColor(backgroundColor);
        // if (fromResource)
        //     WebKitWebContext.GetDefault().RegisterUriScheme("res", OnResRequest);
        if (onAlert != null)
            webWindow.WebView.OnAlert((w, s) => onAlert(s ?? ""));

        webWindow.WebView.OnLoadChanged(OnLoad);
        webWindow.WebView.LoadUri(GetUrl());

        webWindow.Window.Show();
        webWindow.WebView.GrabFocus();
    }

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
}

#endif

