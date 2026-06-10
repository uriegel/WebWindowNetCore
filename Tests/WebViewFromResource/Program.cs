using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .WithDiagnostics()
    .Title("Web Window from Resource 👍")
    .InitialBounds(600, 800)
    .SaveBounds()
    .DefaultContextMenuDisabled()
#if Windows
    .ResourceIcon("icon")
#endif
    .FromResource()
    .QueryString("?param1=123&param2=456")
    .CanClose(() => true)
    .Run();

