using WebWindowNetCore;
using CsTools.Extensions;

static Task<Contact> GetContact(Input text)
    => Task.FromResult(new Contact("Uwe Riegel", 9865));

static Task<Contact2> GetContact2(Input2 text)
    => Task.FromResult(new Contact2("Uwe Riegel", "0177622111"));

static async Task GetImage(Microsoft.AspNetCore.Http.HttpContext context) 
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), context.Request.Query["path"].ToString());
    using var stream = File.OpenRead(path);
    await stream.CopyToAsync(context.Response.Body, 8192);
}

new WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("Web Window Net Core 👍")
    .ResourceIcon("icon")
    .SaveBounds()
    .DefaultContextMenuDisabled()
    .CorsDomains(["*"])
    .AddRequest<Empty, CurrentDirectory>(
        "getCurrentDir", _ => Task.FromResult(new CurrentDirectory(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar)))
    .AddRequest<Input, Contact>("cmd1", GetContact)
    .AddRequest<Input2, Contact2>("cmd2", GetContact2)
    .RequestsDelegates([GetImage])
#if DEBUG    
    .DevTools()
#endif
    //.DebugUrl("https://www.google.de")
    .Url("res://webroot/index.html")
    .CanClose(() => true)
    .OnStarted(action => action.ExecuteJavascript ("console.log('app started now ')"))
    .OnEventSink((id, webView) => 
        new Thread(() =>
        {
            while (true)
            {
                webView.SendEvent(id, new Event($"A new event for {id}"));
                Thread.Sleep(id == "slow" ? 10_000 : 1000);
            }
        })
            .SideEffect(t => t.IsBackground = true)
            .Start()
    )
    .Run();

record Empty();
record CurrentDirectory(string Directory);
record Input(string Text, int Id);
record Contact(string Name, int Id);
record Input2(string EMail, int Count, int Nr);
record Contact2(string DisplayName, string Phone);
record Started(string Name);
record Event(string Text);