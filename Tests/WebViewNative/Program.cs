using WebWindowNetCore;

WebWindow
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
#elif Windows
    .ResourceIcon("icon.ico")
    .WithoutNativeTitlebar()
    .QueryString("?platform=windows")
#endif        
    .Build()
    .Run();

void OnMessage(WebWindow window, string msg)
{
    switch (msg)
    {
        case "maximize":
            window.Maximize();
            break;
        case "minimize":
            window.Minimize();
            break;
        case "restore":
            window.Restore();
            break;
        case "devtools":
            window.ShowDevTools();
            break;
        case "ready": 
            OnStateChange(window);
            break;
    }
}

void OnStateChange(WebWindow window)
    => window.RunJavascript($"onMaximized({(window.IsMaximized ? "true" : "false")})");

