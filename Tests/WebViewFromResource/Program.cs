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
    .FromResource()
    .Build();

webWindow.Run();

