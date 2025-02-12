using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CsTools.Extensions;
using WebWindowNetCore;

var names = Assembly.GetEntryAssembly().GetManifestResourceNames();

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("React WebView 👍")
    .InitialBounds(600, 800)
    .SaveBounds()
    .DevTools()
    .DefaultContextMenuDisabled()
    .BackgroundColor(Color.Transparent)
#if Windows    
    .ResourceIcon("icon")
#endif    
    //.DebugUrl("http://localhost:5173")
    .Url("res://react.test")
    .QueryString("?param1=123&param2=456")
    .CanClose(() => true)
    .Run();

record Input(string Text, int Id);
record Contact(string Name, int Id);

//     //.Url("res://webroot/index.html")
//     .CorsDomains([|"http://localhost:5173"|])
//     .CorsCache(TimeSpan.FromSeconds(20))
//     .AddRequest<Input, Contact>("cmd1", getContact)
//     .DevTools()
