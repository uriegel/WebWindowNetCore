open System
open WebWindowNetCore

[<EntryPoint>]
let main argv =

    let iconPath = @"C:\Users\urieg\source\repos\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico"
    //let url = @"file://C:\Users\urieg\source\repos\WebWindowNetCore\WebRoot\index.html"
    let url = @"file:///media/speicher/projekte/WebWindowNetCore/WebRoot/index.html"
    // let url = "https://google.de"

    let callback (text: string) =
            printfn "Das kam vom lieben Webview: %s" text
            let t = text
            ()
    let callbackDelegate = WebWindow.Callback callback

    let configuration = { 
        WebWindow.defaultConfiguration () with
            title = "Web brauser😎😎👌"
            url = url
            iconPath = iconPath
            debuggingEnabled = true
            organization = "URiegel"
            application = "TestBrauser"
            saveWindowSettings = true
            fullScreenEnabled = true
            callback = callbackDelegate
    }

    WebWindow.initialize configuration

    async {
        let rec readLine () = 
            let line = Console.ReadLine ()
            WebWindow.sendToBrowser line
            readLine ()
        readLine ()
    } |> Async.Start

    WebWindow.execute ()
