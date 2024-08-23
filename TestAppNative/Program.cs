using System.Drawing;
using WebWindowNetCore;

new WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("Web Window Net Core Native Extensions👍")
    .BackgroundColor(Color.Transparent)
    .ResourceIcon("icon")
    .SaveBounds()
    .DefaultContextMenuDisabled()
#if DEBUG    
    .DevTools()
#endif
    .WithoutNativeTitlebar()
    .Url("res://webroot/index.html")
    //.Url($"file://{Directory.GetCurrentDirectory()}/TestAppNative/webroot/index.html")
#if Linux
    .TitleBar(Titlebar.Create)
#elif Windows
    .OnFormCreating(WindowsExtensions.FormCreation)
    .OnHamburger(WindowsExtensions.OnHamburger)
    .OnFilesDrop(WindowsExtensions.OnFilesDrop)
#endif
    .Run();

