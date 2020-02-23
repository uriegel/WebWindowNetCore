// Learn more about F# at http://fsharp.org

open System
open System.Runtime.InteropServices

[<Literal>]
let private DllName = "NativeWinWebView"

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
    end


[<DllImport(DllName, EntryPoint = "initialize_window", CallingConvention = CallingConvention.Cdecl)>] 
extern void initialize (Configuration configuration)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern int execute ()

[<EntryPoint>]
let main argv =
    printfn "Hello World from new F#!"
    initialize (Configuration(title = "Web Brauser😎😎👌", url = "https://www.caseris.de", iconPath = @"C:\Users\urieg\source\repos\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico",
                                debuggingEnabled = true, debuggingPort = 0, organization = "URiegel", application = "TestBrauser", saveWindowSettings = true, fullScreenEnabled = false))
    execute ()
