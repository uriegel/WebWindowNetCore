open WebWindowNetCore

let canClose () = false

WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("F# WebView")
    .Url("https://google.de")
    .SaveBounds()
    .CanClose(canClose)
    .Run()
    |> ignore

