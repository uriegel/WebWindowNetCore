using System.Drawing;
using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core 👍")
    .InitialBounds(600, 800)
    .SaveBounds()
    .BackgroundColor(Color.Transparent)
    .Url("https://github.com/uriegel/WebWindowNetCore")
    .Run();

Console.WriteLine("Hallo");