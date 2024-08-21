open WebWindowNetCore
open System
open System.IO
open Requests

type Input = { Text: string; Id: int }
type Contact = { Name: string; Id: int }
let getContact (text: Input) =
    task { return { Name = "Uwe Riegel"; Id = 9865 } }  

let onRequest (method: string) (input: Stream) =
    task {   
        return! 
            match method with
            | "cmd1" -> input |> GetInput |> getContact |> AsTask
            | _ -> task { return obj() }
    }
 
WebView()
    .AppId("de.uriegel.test")
    .Title("React WebView")
    .ResourceWebroot("webroot")
    .DebugUrl("http://localhost:5173")
    .SaveBounds()
    //.Url("res://webroot/index.html")
    .DefaultContextMenuDisabled()
    .CorsDomains([|"http://localhost:5173"|])
    .CorsCache(TimeSpan.FromSeconds(20))
    .AddRequest<Input, Contact>("cmd1", getContact)
    .DevTools()
    .Run()
    |> ignore