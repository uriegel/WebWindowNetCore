using System.Drawing;
using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core 👍")
    .InitialBounds(600, 800)
    .SaveBounds()
    .DevTools()
    //.DefaultContextMenuDisabled()
    .BackgroundColor(Color.Transparent)
    //.DebugUrl("https://github.com/uriegel/WebWindowNetCore")
    .Url("res://webroot/index.html")
    .QueryString("?param1=123&param2=456")
    .Run();