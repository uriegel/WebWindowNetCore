#if Linux

using Gtk4DotNet;
using WebWindowNetCore;

class LinuxWindow : AdwApplicationWindow
{
    public static ApplicationWindow OnActivation(WebWindow webWindow, WindowBuilder builder)
        => new LinuxWindow(webWindow, builder);

    public LinuxWindow(WebWindow webWindow, WindowBuilder builder) : base(builder)
    {
        AddActions(
            new SimpleAction("quit", CloseWindow, "<Ctrl>Q"),
            new SimpleAction("devtools", webWindow.ShowDevTools, "F12")
        );
    }
}

#endif