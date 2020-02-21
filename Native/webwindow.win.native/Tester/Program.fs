// Learn more about F# at http://fsharp.org

open System
open System.Runtime.InteropServices

[<Literal>]
let private DllName = "NativeWinWebView.dll"

[<StructLayout(LayoutKind.Sequential)>]
type Configuration = 
    struct 
        [<MarshalAs(UnmanagedType.LPWStr)>]
        val mutable title: string
        [<MarshalAs(UnmanagedType.LPWStr)>]
        val mutable url: string
    end


[<DllImport(DllName, EntryPoint = "initialize_window", CallingConvention = CallingConvention.Cdecl)>] 
extern void initialize (Configuration configuration)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern int execute ()

[<EntryPoint>]
let main argv =
    printfn "Hello World from new F#!"
    initialize (Configuration(title = "Webbrauser😎😎👌", url = "https://www.caseris.de"))
    execute ()
