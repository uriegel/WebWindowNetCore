using System.Drawing;
using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core Native 👍")
#if Linux    
    .WithBuilder(TestAppNative.Linux.WebView.WithBuilder)
#endif    
    .OnFilesDrop((id, move, files) => WebView.RunJavascript("alert(`Id: ${id}, move: ${move}, files: ${files}`)"))
    .InitialBounds(1200, 800)
    .ResourceIcon("icon")
    .SaveBounds()
    .DevTools()
    .DefaultContextMenuDisabled()
    .BackgroundColor(Color.Transparent)
    .Url("res://webroot/index.html")
    .CanClose(() => true)
    .Run();

//     .WithoutNativeTitlebar()
//     .Url("res://webroot/index.html")
//     //.Url($"file://{Directory.GetCurrentDirectory()}/TestAppNative/webroot/index.html")
// #if Linux
//     .TitleBar(Titlebar.Create)
// #elif Windows
//     .OnFormCreating(WindowsExtensions.FormCreation)
//     .OnHamburger(WindowsExtensions.OnHamburger)

// #endif
//     .Run();

