using System.Drawing;
using WebWindowNetCore;


var lgn = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core 👍")
    .InitialBounds(600, 800)
    .SaveBounds()
    .BackgroundColor(Color.Transparent)
    .Url("res://webroot/index.html")
    .Run();

Console.WriteLine("Hallo");