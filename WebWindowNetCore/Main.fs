namespace WebWindowNetCore

open System.Runtime.InteropServices

module WebWindow = 
    
    [<Literal>]
    let private DllName = "libminimal.so.1.0.0"

    [<DllImport(DllName, EntryPoint = "run_web_window", CallingConvention = CallingConvention.Cdecl)>] 
    extern void runWebWindow (string url)

    let Run url = 
        runWebWindow url
