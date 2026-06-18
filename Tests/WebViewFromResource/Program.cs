using WebWindowNetCore;

var webWindow = WebWindow
    .Builder()
    .AppId("de.uriegel.test")
    .Title("Hello Web Window👍")
    .WithDiagnostics(true)
    .InitialBounds(600, 800)
    .SaveBounds()
    .DefaultContextMenuDisabled()
#if Windows
    .ResourceIcon("icon")
#endif
    .FromResource()
    .Build();

webWindow.Run();

