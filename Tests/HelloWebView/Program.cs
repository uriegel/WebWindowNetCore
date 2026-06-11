using WebWindowNetCore;

var webWindow = WebView
    .Builder()
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
    .Build();

webWindow.Run();


