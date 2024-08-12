open WebWindowNetCore
open System.IO

let canClose () = true

WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("F# WebView")
    .ResourceScheme()
    .Url(sprintf "file://%s/webroot/index.html" (Directory.GetCurrentDirectory ()))
    .SaveBounds()
    .DefaultContextMenuDisabled()
#if DEBUG    
    .DevTools()
#endif
    .CanClose(canClose)
    .Run()
    |> ignore

