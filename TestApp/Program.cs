using WebWindowNetCore;

new WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("Web Window Net Core 👍")
    .ResourceIcon("icon")
    .ResourceScheme()
    .SaveBounds()
    .DefaultContextMenuDisabled()
#if DEBUG    
    .DevTools()
#endif
    //.DebugUrl("https://www.google.de")
    .Url("res://webroot/index.html")
    .CanClose(() => true)
    .Run();

//     .ConfigureHttp(http => http
//         .ResourceWebroot("webroot", "/webroot")
//         //.MapGet("requests/icon", context => context.SendStream()))
//         .JsonPost<Cmd1Param, Cmd1Result>("request/cmd1", JsonRequest1)
//         .JsonPost<Cmd2Param, Cmd2Result>("request/cmd2", JsonRequest2)
//         .UseSse("sse/test", sseEventSource)
//         .Build())


// Task<Cmd1Result> JsonRequest1(Cmd1Param param)
//     => new Cmd1Result("Result", 3).ToAsync(); 

// Task<Cmd2Result> JsonRequest2(Cmd2Param param)
//     => new Cmd2Result("Result 2", 32).ToAsync(); 

// record Event(string Content);

// record Cmd1Param(string Text, int Id);
// record Cmd1Result(string Result, int Id);
// record Cmd2Param(string Name, int Number);
// record Cmd2Result(string Name, int Number);
