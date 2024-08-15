open WebWindowNetCore
open System.IO
open Requests

let canClose () = true

type Input = { text: string; id: int }
type Contact = { name: string; id: int }

let getContact (text: Input) =
    task {
        return { name = "Uwe Riegel"; id = 9865 }
    }  

let onRequest (method: string) (input: Stream) =
    task {   
        return! 
            match method with
            | "cmd1" -> input |> GetInput |> getContact |> AsTask
            | _ -> task { return obj() }
    }

// TODO: asynchronous XmlHttpRequest in Windows
// TODO: asynchronous XmlHttpRequest in Linux

WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("F# WebView")
    .ResourceScheme()
    .Url(sprintf "file://%s/webroot/index.html" (Directory.GetCurrentDirectory ()))
    .SaveBounds()
    .DefaultContextMenuDisabled()
    //.AddRequest(onRequest)
#if DEBUG    
    .DevTools()
#endif
    .CanClose(canClose)
    .Run()
    |> ignore

