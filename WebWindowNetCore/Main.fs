namespace WebWindowNetCore

open System.Runtime.InteropServices
open System.Threading

module WebWindow = 
    
#if Linux 
    [<Literal>]
    let private DllName = "libwebwindow.so.1.0.0"
#else
    [<Literal>]
    let private DllName = "webwindow.dll"
#endif            

    [<DllImport(DllName, EntryPoint = "run_web_window", CallingConvention = CallingConvention.Cdecl)>] 
    extern void runWebWindow (string url)
    
    let Run url = 

        let t = Thread (fun () -> runWebWindow url)
#if Windows
        t.SetApartmentState(ApartmentState.STA)
#endif        
        t.Start()
        t.Join()
        
