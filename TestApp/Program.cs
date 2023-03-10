WebView
    .Create()
    .InitialBounds(600, 800)
    .Title("Commander")
    .SaveBounds()
    //.Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
    .ConfigureHttp(http => http
        .ResourceWebroot("webroot", "/web")
        .Build())
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run("de.uriegel.Commander");    
