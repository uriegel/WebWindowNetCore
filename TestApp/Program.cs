using LinqTools;

var sseEventSource = WebView.CreateEventSource<Event>();
StartEvents(sseEventSource.Send);

WebView
    .Create()
    .InitialBounds(600, 800)
    .Title("WebView Test")
    .ResourceIcon("icon")
    .SaveBounds()
    //.DebugUrl("https://www.google.de")
    .DebugUrl("http://localhost:19999/cinema")
    //.Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
    .ConfigureHttp(http => http
        .ResourceWebroot("webroot", "/web")
        .UseJsonPost<Cmd1Param, Cmd1Result>("request/cmd1", JsonRequest1)
        .UseJsonPost<Cmd2Param, Cmd2Result>("request/cmd2", JsonRequest2)
        .UseSse("sse/test", sseEventSource)
        .Build())
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run("de.uriegel.Commander");    

void StartEvents(Action<Event> onChanged)   
{
    var counter = 0;
    new Thread(_ =>
        {
            while (true)
            {
                Thread.Sleep(5000);
                onChanged(new($"Ein Event {counter++}"));
           }
        })
        {
            IsBackground = true
        }.Start();   
}

Task<Cmd1Result> JsonRequest1(Cmd1Param param)
    => new Cmd1Result("Result", 3).ToAsync(); 

Task<Cmd2Result> JsonRequest2(Cmd2Param param)
    => new Cmd2Result("Result 2", 32).ToAsync(); 

record Event(string Content);

record Cmd1Param(string Text, int Id);
record Cmd1Result(string Result, int Id);
record Cmd2Param(string Name, int Number);
record Cmd2Result(string Name, int Number);
