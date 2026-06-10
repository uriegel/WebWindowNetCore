using CsTools.Extensions;
using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .WithDiagnostics()
#if Linux    
    .FromResourceTemplate("template", LinuxWindow.OnActivation, true)
    .SideEffect(w => LinuxWindow.WebView = w)
#endif    
    .Title("Web Window with native extensions👍")
    .InitialBounds(600, 800)
    .SaveBounds()
    .DevTools()
    .DefaultContextMenuDisabled()
#if Windows
    .ResourceIcon("icon")
#endif
    .FromResource()
    .QueryString("?param1=123&param2=456")
    .CanClose(() => true)
    .Run();

