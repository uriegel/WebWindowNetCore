using WebWindowNetCore;
#if Linux
using CsTools.Extensions;
#endif

WebView
    .Create()
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
    .InitialBounds(600, 800)
    .SaveBounds()
    .DevTools()
   // .DefaultContextMenuDisabled()
    .FromResource()
    .CanClose(() => true)
    .Run();

