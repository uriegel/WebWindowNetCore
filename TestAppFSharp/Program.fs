open WebWindowNetCore
open System.IO
open Requests
open System.Threading

let canClose () = true

type Input = { Text: string; Id: int }
type Contact = { Name: string; Id: int }
type Input2 = { EMail: string; Count: int;  Nr: int }
type Contact2 = { DisplayName: string; Phone: string }
type Event = { Text: string }
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

let onStarted (webViewAccess: WebViewAccess) =
    webViewAccess.ExecuteJavascript.Invoke "console.log('app started now ')"

let eventSink id (webView: WebViewAccess) =
    let func () = 
        while true do
            webView.SendEvent.Invoke(id, { Text = (sprintf "A new event for %s" id) })
            Thread.Sleep (if id = "slow" then 10_000 else 1000)

    let t = new Thread(func)
    t.IsBackground <- true
    t.Start ()

WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("F# WebView")
    .Url(sprintf "file://%s/webroot/index.html" (Directory.GetCurrentDirectory ()))
    .SaveBounds()
    .DefaultContextMenuDisabled()
    .AddRequest<Input, Contact>("cmd1", getContact)
    .AddRequest<Input2, Contact2>("cmd2", getContact2)
    .OnStarted(onStarted)
    .OnEventSink(eventSink)
#if DEBUG    
    .DevTools()
#endif
    .CanClose(canClose)
    .Run()
    |> ignore