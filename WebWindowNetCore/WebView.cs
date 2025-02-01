namespace WebWindowNetCore;

public abstract class WebView
{
#if Linux
    public static WebView Create() => new Linux.WebView();
#endif    
}
