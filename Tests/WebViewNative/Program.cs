using WebWindowNetCore;
#if Linux
using CsTools.Extensions;
#endif

var webView = WebView
    .Create();
webView
    .AppId("de.uriegel.test")
    .WithDiagnostics()
#if Linux    
    .FromResourceTemplate("template", LinuxWindow.OnActivation, true)
    .SideEffect(w => LinuxWindow.WebView = w)
#elif Windows
    .ResourceIcon("icon.ico")
    .WithoutNativeTitlebar()
    .QueryString("?platform=windows")
#endif        
    .Title("Web Window with native extensions👍")
    .ScriptDialog(OnMessage)
    .InitialBounds(600, 800)
    .SaveBounds()
    .DevTools()
    .DefaultContextMenuDisabled()
    .FromResource()
    .OnStateChange(OnStateChange)
    .CanClose(() => true);

webView.Run();

void OnMessage(string msg)
{
    switch (msg)
    {
        case "maximize":
            webView.Maximize();
            break;
        case "minimize":
            webView.Minimize();
            break;
        case "restore":
            webView.Restore();
            break;
        case "devtools":
            webView.ShowDevTools();
            break;
        case "ready": 
            OnStateChange();
            break;
    }
}

void OnStateChange()
{
    webView.RunJavascript($"onMaximized({(webView.IsMaximized ? "true" : "false")})");
}
