#if Linux

using Gtk4DotNet;

class LinuxWindow : AdwApplicationWindow
{
    public static WebWindowNetCore.WebView? WebView { get; set; }

    public static ApplicationWindow OnActivation(Application app, WindowBuilder builder)
        => new LinuxWindow(builder);

    public LinuxWindow(WindowBuilder builder) : base(builder)
    {
        this.AddActions([
            new("quit", CloseWindow, "<Ctrl>Q"),
            new("devtools", () => WebView?.ShowDevTools(), "F12")
        ]);
    }

    [Widget(Name="webview")]
    readonly WebView webView = null!;
}

#endif