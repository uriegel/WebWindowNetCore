open WebWindowNetCore
open System.IO
open Requests

let canClose () = true

type Input = { Text: string; Id: int }
type Contact = { Name: string; Id: int }
type Input2 = { EMail: string; Count: int;  Nr: int }
type Contact2 = { DisplayName: string; Phone: string }

let getContact (text: Input) =
    task { return { Name = "Uwe Riegel"; Id = 9865 } }  

let getContact2 (text: Input2) =
    task { return { DisplayName = "Uwe Riegel"; Phone = "0177622111" } }  

let onRequest (method: string) (input: Stream) =
    task {   
        return! 
            match method with
            | "cmd1" -> input |> GetInput |> getContact |> AsTask
            | _ -> task { return obj() }
    }

WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("F# WebView")
    .ResourceScheme()
    .Url(sprintf "file://%s/webroot/index.html" (Directory.GetCurrentDirectory ()))
    .SaveBounds()
    .DefaultContextMenuDisabled()
    .AddRequest<Input, Contact>("cmd1", getContact)
    .AddRequest<Input2, Contact2>("cmd2", getContact2)
#if DEBUG    
    .DevTools()
#endif
    .CanClose(canClose)
    .Run()
    |> ignore

