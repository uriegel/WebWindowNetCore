using System.Drawing;
using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core Native 👍")
#if Linux    
    .WithBuilder(TestAppNative.Linux.WebView.WithBuilder)
#elif Windows
    .ResourceIcon("icon")
    .WithoutNativeTitlebar()
#endif        
    .InitialBounds(1200, 800)
    .SaveBounds()
    .DevTools()
    .DefaultContextMenuDisabled()
    .BackgroundColor(Color.Transparent)
    .Url("res://webroot/index.html")
    .CanClose(() => true)
    .Run();
//     //.Url($"file://{Directory.GetCurrentDirectory()}/TestAppNative/webroot/index.html")
// #elif Windows
//     .OnFormCreating(WindowsExtensions.FormCreation)


