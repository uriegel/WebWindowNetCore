using WebWindowNetCore;
#if Linux
using CsTools.Extensions;
#endif

var webWindow = WebView
    .Builder()
    .AppId("de.uriegel.test")
    .Title("Web Window with native extensions👍")
    .WithDiagnostics()
    .InitialBounds(600, 800)
    .SaveBounds()
    .DevTools()
    .FromResource()
    .DefaultContextMenuDisabled()
    .OnStateChange(OnStateChange)
    .ScriptDialog(OnMessage) 
    .CanClose(() => true)
#if Linux    
    .FromResourceTemplate("template", LinuxWindow.OnActivation, true)
#endif
#if Windows
    .ResourceIcon("icon")
#endif
    .Build();

webWindow.Run();

void OnMessage(string msg)
{
    switch (msg)
    {
        case "maximize":
            //webWindow.Maximize();
            break;
        case "minimize":
            //webWindow.Minimize();
            break;
        case "restore":
            //webWindow.Restore();
            break;
        case "devtools":
            //webWindow.ShowDevTools();
            break;
        case "ready": 
            OnStateChange();
            break;
    }
}

void OnStateChange()
{
    // TODO webView.RunJavascript($"onMaximized({(webView.IsMaximized ? "true" : "false")})");
}
