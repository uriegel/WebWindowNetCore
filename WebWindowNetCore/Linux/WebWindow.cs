#if Linux

using Gtk4DotNet;

namespace WebWindowNetCore.Linux;

public class WebWindow : WebWindowNetCore.WebWindow
{
    public Application Application { get; }
    public ApplicationWindow Window { get; internal set; } = null!;
    public Gtk4DotNet.WebView WebView { get; internal set; } = null!;

    public override int Run() => Application.Run(0, 0);

    internal WebWindow(Application application) => Application = application;
}

#endif