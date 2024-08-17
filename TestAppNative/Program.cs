using WebWindowNetCore;
#if Linux
using GtkDotNet;
#endif

new WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("Web Window Net Core Native Extensions👍")
    .ResourceIcon("icon")
    .ResourceScheme()
    .SaveBounds()
    .DefaultContextMenuDisabled()
#if DEBUG    
    .DevTools()
#endif
    .WithoutNativeTitlebar()
    .Url("res://webroot/index.html")
#if Linux
    .TitleBar()
#elif Windows
    .OnFormCreating(WindowsExtensions.FormCreation)
#endif
    .Run();

