using AspNetExtensions;

namespace WebWindowNetCore;

public abstract class WebView
{
    public static SseEventSource<T> CreateEventSource<T>()
        => new SseEventSource<T>();
        
    public abstract int Run(string gtkId = "");
}