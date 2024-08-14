using WebWindowNetCore;
using static WebWindowNetCore.Requests;

static async Task<object> OnRequest(string method, Stream input)
    => method switch
    {
        "cmd1" => await GetContact(GetInput<Input>(input)),
        _ => new object()
    };

static Task<Contact> GetContact(Input? text)
    => Task.FromResult(new Contact("Uwe Riegel", 9865));

new WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("Web Window Net Core 👍")
    .ResourceIcon("icon")
    .ResourceScheme()
    .SaveBounds()
    .DefaultContextMenuDisabled()
    .OnRequest(OnRequest)
#if DEBUG    
    .DevTools()
#endif
    //.DebugUrl("https://www.google.de")
    .Url("res://webroot/index.html")
    .CanClose(() => true)
    .Run();

record Input(string text, int id);
record Contact(string Name, int Id);