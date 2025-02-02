namespace WebWindowNetCore;

static class ScriptInjection
{
    public static string Get() => 
$@"

{Linux.ScriptInjection.Get()}

var WebView = (() => {{
    return {{
//        initializeNoTitlebar,
        showDevTools,
//        startDragFiles,
        // request,
        // dropFiles,
        // filesDropped,
        // setDroppedFilesEventHandler,
        // setDroppedEvent,
        // closeWindow,
        // backtothefuture,
        // additionalObjectsBack,
        // startDragFilesBack
    }}
}})()
";
}