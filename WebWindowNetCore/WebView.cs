using AspNetExtensions;

namespace WebWindowNetCore.Base;

public abstract class WebView
{
    public static SseEventSource<T> CreateEventSource<T>()
        => new();
        
    public abstract int Run();
}