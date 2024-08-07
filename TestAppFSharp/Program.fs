open WebWindowNetCore

WebView()
    .AppId("de.uriegel.test")
    .Width(1200)
    .Title("F# WebView")
    .Url("https://google.de")
    .Run()
    |> ignore

