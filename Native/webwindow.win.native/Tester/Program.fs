// Learn more about F# at http://fsharp.org

open System
open System.Runtime.InteropServices

[<Literal>]
let private DllName = "NativeWinWebView"

[<UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet=CharSet.Auto)>]
type Callback = delegate of string -> unit

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)>]
type Configuration = 
    struct 
        val mutable title: string
        val mutable url: string
        val mutable iconPath: string
        [<MarshalAs(UnmanagedType.U1)>]
        val mutable debuggingEnabled: bool
        val mutable debuggingPort: int
        val mutable organization: string
        val mutable application: string
        [<MarshalAs(UnmanagedType.U1)>]
        val mutable saveWindowSettings: bool
        [<MarshalAs(UnmanagedType.U1)>]
        val mutable fullScreenEnabled: bool
        val mutable callback: Callback
    end



[<DllImport(DllName, EntryPoint = "initialize_window", CallingConvention = CallingConvention.Cdecl)>] 
extern void initialize (Configuration configuration)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern int execute ()

[<DllImport(DllName, EntryPoint = "send_to_browser", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)>] 
extern void sendToBrowser (string text)

[<EntryPoint>]
let main argv =
    printfn "Hello World from new F#!"
    let url = @"file://C:\Users\urieg\source\repos\WebWindowNetCore\WebRoot\index.html"
    //let url = "https://google.de"

    let callback (text: string) =
            printfn "Das kam vom lieben Webview: %s" text
            let t = text
            ()
    let callbackDelegate = Callback callback

    initialize (Configuration(title = "Web Brauser😎😎👌", url = url, iconPath = @"C:\Users\urieg\source\repos\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico",
                                debuggingEnabled = true, debuggingPort = 0, organization = "URiegel", application = "TestBrauser", saveWindowSettings = true, fullScreenEnabled = true,
                                callback = callbackDelegate))


    async {
        let rec readLine () = 
            let line = Console.ReadLine ()
            sendToBrowser line
            readLine ()
        readLine ()
    } |> Async.Start

    execute ()
