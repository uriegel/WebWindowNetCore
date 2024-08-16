using CsTools.Extensions;
using WebWindowNetCore;

static Task<Contact> GetContact(Input text)
    => Task.FromResult(new Contact("Uwe Riegel", 9865));

static Task<Contact2> GetContact2(Input2 text)
    => Task.FromResult(new Contact2("Uwe Riegel", "0177622111"));

new WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("Web Window Net Core 👍")
    .ResourceIcon("icon")
    .ResourceScheme()
    .SaveBounds()
    .DefaultContextMenuDisabled()
    .AddRequest<Input, Contact>("cmd1", GetContact)
    .AddRequest<Input2, Contact2>("cmd2", GetContact2)
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

record Input(string Text, int Id);
record Contact(string Name, int Id);
record Input2(string EMail, int Count, int Nr);
record Contact2(string DisplayName, string Phone);
record Started(string Name);
record Event(string Text);