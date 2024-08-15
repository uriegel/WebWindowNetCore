using WebWindowNetCore;

static Task<Contact> GetContact(Input text)
{
    Console.WriteLine("SChrott");
    return Task.FromResult(new Contact("Uwe Riegel", 9865));
}    

static Task<Contact2> GetContact2(Input2 text)
    => Task.FromResult(new Contact2("Uwe Riegel", "9865"));

new WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("Web Window Net Core 👍")
    .ResourceIcon("icon")
    .ResourceScheme()
    .SaveBounds()
    //.DefaultContextMenuDisabled()
    .AddRequest<Input, Contact>("test", GetContact)
    .AddRequest<Input2, Contact2>("test2", GetContact2)
#if DEBUG    
    .DevTools()
#endif
    //.DebugUrl("https://www.google.de")
    .Url("res://webroot/index.html")
    .CanClose(() => true)
    .Run();

record Input(string Text, int Id);
record Contact(string Name, int Id);
record Input2(string Text, int Id, int Nr);
record Contact2(string Name, string Id);