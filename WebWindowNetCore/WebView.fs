namespace WebWindowNetCore

module WebView = 
#if Windows
    let Create (): WebViewBase = WebViewWindows ()
#elif Linux
    let Create (): WebViewBase = WebViewLinux ()
#else
    ()
#endif
    let Create (): WebViewBase = 
#if Windows
        WebViewWindows ()
#else 
        WebViewLinux ()
#endif

    