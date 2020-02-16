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

#if DEBUG 
    let private debug_mode = true
#else
    let private debug_mode = false
#endif

    [<DllImport(DllName, EntryPoint = "run_web_window", CallingConvention = CallingConvention.Cdecl)>] 
    extern void runWebWindow (string url, [<MarshalAs(UnmanagedType.I1)>] bool debugMode)
    
    let Run url = 

        // TODO: sudo ./minibrowser --remote-debugging-port=8888

        let t = Thread (fun () -> runWebWindow url)
#if Windows
        t.SetApartmentState(ApartmentState.STA)
#endif        
        t.Start()
        t.Join()
        
