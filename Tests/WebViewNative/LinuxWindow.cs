#if Linux

using Gtk4DotNet;
using WebWindowNetCore;

class LinuxWindow : AdwApplicationWindow
{
    public static ApplicationWindow OnActivation(WebWindow webWindow, WindowBuilder builder)
        => new LinuxWindow(webWindow, builder);

    public LinuxWindow(WebWindow webWindow, WindowBuilder builder) : base(builder)
    {
        this.AddActions([
            new("quit", CloseWindow, "<Ctrl>Q"),
            new("devtools", webWindow.ShowDevTools, "F12")
        ]);
    }
}

#endif