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
        //year:uint16
        //month:uint16
        //weekday:uint16
        //day:uint16
        //hour:uint16
        //minute:uint16
        val mutable second:uint16
        //millisecond:uint16
    end


[<DllImport(DllName, EntryPoint = "initialize_window", CallingConvention = CallingConvention.Cdecl)>] 
extern void initialize (Configuration configuration)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern int execute ()

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    initialize (new Configuration(title = "Webbrauser😎😎👌", second = 34us))
    execute ()
