using System.Drawing;
using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core 👍")
    .InitialBounds(600, 800)
    .SaveBounds()
    .DevTools()
    .BackgroundColor(Color.Transparent)
    .Url("res://webroot/index.html")
    .Run();

Console.WriteLine("Hallo");