namespace WebWindowNetCore;

public record Point(int X, int Y);
public record Size(int Width, int Height);
public record Configuration(
    string Title = "WebWindowNetCore", 
    string Url = "https://github.com/", 
    string Organization = "URiegel",
    string Application = "WebWindowNetCore",
    string IconPath = "", 
    bool DebuggingEnabled = false,
    bool FullscreenEnabled = false,
    bool IsMaximized = false,
    bool SaveWindowSettings = false,
    Point? InitialPosition = null,
    Size? InitialSize = null
);
