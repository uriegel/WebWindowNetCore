using System.Drawing;
using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core Native 👍")
#if Linux    
    .WithBuilder(TestAppNative.Linux.WebView.WithBuilder)
#endif    
    .InitialBounds(600, 800)
    .SaveBounds()
    .DevTools()
    .DefaultContextMenuDisabled()
    .BackgroundColor(Color.Transparent)
    .Url("res://webroot/index.html")
    .CanClose(() => true)
    .Run();

// new WebView()
//     .AppId("de.uriegel.test")
//     .InitialBounds(1200, 800)
//     .Title("Web Window Net Core Native Extensions👍")
//     .BackgroundColor(Color.Transparent)
//     .ResourceIcon("icon")
//     .SaveBounds()
//     .DefaultContextMenuDisabled()
// #if DEBUG    
//     .DevTools()
// #endif
//     .WithoutNativeTitlebar()
//     .Url("res://webroot/index.html")
//     //.Url($"file://{Directory.GetCurrentDirectory()}/TestAppNative/webroot/index.html")
// #if Linux
//     .TitleBar(Titlebar.Create)
// #elif Windows
//     .OnFormCreating(WindowsExtensions.FormCreation)
//     .OnHamburger(WindowsExtensions.OnHamburger)
//     .OnFilesDrop(WindowsExtensions.OnFilesDrop)
// #endif
//     .Run();

