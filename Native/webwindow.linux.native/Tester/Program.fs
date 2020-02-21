// Learn more about F# at http://fsharp.org

open System
open System.Runtime.InteropServices

[<Literal>]
let private DllName = "libwebwindowlinuxnative.so.1.0.0"

[<StructLayout(LayoutKind.Sequential)>]
type Configuration = 
    struct 
        [<MarshalAs(UnmanagedType.LPUTF8Str)>]
        val mutable title: string
        [<MarshalAs(UnmanagedType.LPUTF8Str)>]
        val mutable url: string
    end


[<DllImport(DllName, EntryPoint = "initialize_window", CallingConvention = CallingConvention.Cdecl)>] 
extern void initialize (Configuration configuration)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern int execute ()

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    initialize (new Configuration(title = "Webbrauser😎😎👌", url = "www.google.de"))
    execute ()
