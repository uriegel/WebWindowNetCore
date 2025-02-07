using System.Drawing;
using System.Reflection;
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
    .Url("res://react.test/index.html")
    .QueryString("?param1=123&param2=456")
    .OnRequest(OnRequest)
    .CanClose(() => true)
    .Run();

void OnRequest(Request request)
{
    switch (request.Cmd)
    {
        case "cmd1":
            {
                var data = request.Deserialize<Input>();
                request.Response(new Contact("Uwe Riegel", 9865));
            }
            break;
    }
}

record Input(string Text, int Id);
record Contact(string Name, int Id);

//     //.Url("res://webroot/index.html")
//     .CorsDomains([|"http://localhost:5173"|])
//     .CorsCache(TimeSpan.FromSeconds(20))
//     .AddRequest<Input, Contact>("cmd1", getContact)
//     .DevTools()
