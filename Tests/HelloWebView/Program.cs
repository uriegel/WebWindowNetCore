using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Hello Web Window👍")
    .WithDiagnostics()
    .InitialBounds(600, 800)
    .SaveBounds()
    .DefaultContextMenuDisabled()
#if Windows
    .ResourceIcon("icon")
#endif
    .DebugUrl("https://github.com/uriegel/WebWindowNetCore")
    .Url("https://github.com")
    .QueryString("?param1=123&param2=456")
    .CanClose(() => true)
    .Run();

