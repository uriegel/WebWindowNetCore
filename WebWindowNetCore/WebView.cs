using AspNetExtensions;

namespace WebWindowNetCore;

public class Test2
{
#if Windows
    public static string RunWindows()
        => "Ist Windows";
#endif

#if Linux
    public static string RunLinux()
        => "Ist Linux";
#endif        
  
}

// public abstract class WebView
// {
//     public static SseEventSource<T> CreateEventSource<T>()
//         => new();

//     public abstract int Run();
// }