WebView
    .Create()
    .InitialBounds(600, 800)
    .Title("WebView Test")
    .SaveBounds()
    //.Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
    .ConfigureHttp(http => http
        .ResourceWebroot("webroot", "/web")
        .ResourceFavicon("favicon")
        .Build())
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run("de.uriegel.Commander");    
