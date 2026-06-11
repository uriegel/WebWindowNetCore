#if Linux

using Gtk4DotNet;

namespace WebWindowNetCore.Linux;

// TODO Titlebar: Linux: buttons in content for controlling
// TODO CheckDiagnostics: FromResource 1 delegate remaining
// TODO UnregisterUriScheme

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
        webWindow.Window = resourceTemplate != null && onActivate != null
            ? app.WithWebKit().WindowFromBuilder(resourceTemplate, "window", builder =>
            {
                var window = onActivate(webWindow, builder);
                webWindow.WebView = builder.Builder.GetWidget<Gtk4DotNet.WebView>("webview");
                return window;
            })
            : app.NewWindow();

        webWindow.Window.Title = title;
        if (saveBounds)
            WebWindow.WithSaveBounds(webWindow.Window, appId, width, height);
        else
            webWindow.Window.DefaultSize(width, height);
        if (onStateChanged != null)
            webWindow.Window.OnNotify("maximized", () => onStateChanged(webWindow));
        if (webWindow.WebView == null)
        {
            webWindow.WebView = Gtk4DotNet.WebView.New();
            webWindow.Window.Child(webWindow.WebView);
        }
        if (canClose != null)
            webWindow.Window.OnClose(_ => canClose() == false);

        webWindow.WebView.Visible = false;
        if (devTools)
            webWindow.WebView.GetSettings().EnableDeveloperExtras = true;
        if (defaultContextMenuDisabled)
            webWindow.WebView.DisableContextMenu();
        webWindow.WebView.BackgroundColor(backgroundColor);
        if (fromResource)
            WebKitWebContext.GetDefault().RegisterUriScheme("res", WebWindow.OnResRequest);
        if (onAlert != null)
            webWindow.WebView.OnAlert((w, s) => onAlert(webWindow, s ?? ""));

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
}

#endif

