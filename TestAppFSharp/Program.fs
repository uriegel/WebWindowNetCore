open System.IO
open System.Drawing
open System.Threading
open WebWindowNetCore
open Requests
open Giraffe
open FSharpTools

let canClose () = true

type Input = { Text: string; Id: int }
type Contact = { Name: string; Id: int }
type Input2 = { EMail: string; Count: int;  Nr: int }
type Contact2 = { DisplayName: string; Phone: string }
type Event = { Text: string }
[<CLIMutable>]
type FileRequest = { Path: string }
type Empty = { Nil: int }
type CurrentDirectory = { Directory: string }
type Error = {Code: int }
type JsonResult<'a, 'b> = { Ok: 'a option; Error: 'b option }
let getContact (text: Input) =
    task { return { Name = "Uwe Riegel"; Id = 9865 } }  

let getContact2 (i: Empty) =
    task { 
        let affe = { Ok = Some { DisplayName = "Uwe Riegel"; Phone = "0177622111" } ; Error = None } 
        return affe
    }

let onRequest (method: string) (input: Stream) =
    task {   
        return! 
            match method with
            | "cmd1" -> input |> GetInput |> getContact |> AsTask
            | _ -> task { return obj() }
    }

let onStarted (webViewAccess: WebViewAccess) =
    webViewAccess.ExecuteJavascript.Invoke "console.log('app started now ')"

let getCurrentDirectory (_: Empty) = 
    task {
        return { Directory = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar.ToString() }
    }

let eventSink id (webView: WebViewAccess) =
    let func () = 
        while true do
            webView.SendEvent.Invoke(id, { Text = (sprintf "A new event for %s" id) })
            Thread.Sleep (if id = "slow" then 10_000 else 1000)

    let t = new Thread(func)
    t.IsBackground <- true
    t.Start ()

let getImage =
    let getFile fileRequest = 
        let path = 
            fileRequest.Path
            |> Directory.combine2Pathes (Directory.GetCurrentDirectory ())
        streamFile false path None None
    route "/get/image" >=> bindQuery<FileRequest> None getFile

WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("F# WebView")
    .BackgroundColor(Color.Transparent)
    .Url(sprintf "file://%s/webroot/index.html" (Directory.GetCurrentDirectory ()))
    .CorsDomains([|"*"|])
    .SaveBounds()
    .DefaultContextMenuDisabled()
    .AddRequest("cmd1", getContact)
    .AddRequest("cmd2", getContact2)
    .AddRequest<Empty, CurrentDirectory>("getCurrentDir", getCurrentDirectory)
    .Requests([getImage])
    .OnStarted(onStarted)
    .OnEventSink(eventSink)
#if DEBUG    
    .DevTools()
#endif
    .CanClose(canClose)
    .Run()
    |> ignore