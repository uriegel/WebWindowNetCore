using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core 👍")
    .InitialBounds(600, 800)
    .SaveBounds()
    .Run();

Console.WriteLine("Hallo");