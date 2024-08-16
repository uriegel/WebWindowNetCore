using WebWindowNetCore;

new WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("Web Window Net Core 👍")
    .ResourceIcon("icon")
    .ResourceScheme()
    .SaveBounds()
    .DefaultContextMenuDisabled()
#if DEBUG    
    .DevTools()
#endif
    .Url("res://webroot/index.html")
    .Run();
