using System.Drawing;
using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core 👍")
    .InitialBounds(600, 800)
    .SaveBounds()
    .DevTools()
    .DefaultContextMenuDisabled()
    .BackgroundColor(Color.Transparent)
    //.DebugUrl("https://github.com/uriegel/WebWindowNetCore")
    .Url("res://webroot/index.html")
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
        }
        break;
        case "cmd2":
        {
            var data = request.Deserialize<Input2>();
        }
        break;
    }
}

record Input(string Text, int Id);
record Contact(string Name, int Id);
record Input2(string EMail, int Count, int Nr);
record Contact2(string DisplayName, string Phone);
