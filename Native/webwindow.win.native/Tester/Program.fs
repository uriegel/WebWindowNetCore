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
    end


[<DllImport(DllName, EntryPoint = "initialize_window", CallingConvention = CallingConvention.Cdecl)>] 
extern void initialize (Configuration configuration)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern int execute ()

[<EntryPoint>]
let main argv =
    printfn "Hello World from new F#!"
    initialize (Configuration(title = "Web Brauser😎😎👌", url = "https://www.caseris.de"))
    execute ()
