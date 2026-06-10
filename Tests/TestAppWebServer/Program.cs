using WebServerLight;
using WebWindowNetCore;

var webView = WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core from Web Server 👍")
    .InitialBounds(600, 800)
    .SaveBounds()
    .DevTools()
    .DefaultContextMenuDisabled()
#if Windows
    .ResourceIcon("icon")
#endif
    .Url("http://localhost:8080")
    .QueryString("?param1=123&param2=456")
    .CanClose(() => true);

var server =
    ServerBuilder
        .New()
        .Http(8080)
        .WebsiteFromResource()
        .JsonPost(JsonPost)
        .Build();

server.Start();
webView.Run();
server.Stop();

async Task<bool> JsonPost(JsonRequest request)
{
    switch (request.Url)
    {
        case "/json/showdevtools":
            webView.ShowDevTools();
            await request.SendAsync("{}");
            break;
        case "/json/startdragfiles":
            await webView.StartDragFiles(["file:///test1/test.png"]);
            await request.SendAsync("{}");
            break;
        default:
            return false;
    }
    return true;
}