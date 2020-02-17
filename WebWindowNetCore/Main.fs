namespace WebWindowNetCore

open System.Runtime.InteropServices
open System.Threading
open System
open System.Text

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

    // TODO: To debug: Chrome: localhost:8888

    [<UnmanagedFunctionPointer(CallingConvention.Cdecl)>]
    type Callback = delegate of string -> unit

    [<DllImport(DllName, EntryPoint = "run_web_window", CallingConvention = CallingConvention.Cdecl)>] 
    extern void runWebWindow (string url, [<MarshalAs(UnmanagedType.I1)>] bool debugMode)

    [<DllImport(DllName, EntryPoint = "test_callback", CallingConvention = CallingConvention.Cdecl)>] 
    extern void testCallback ()

    [<DllImport(DllName, EntryPoint = "init_callback", CallingConvention = CallingConvention.Cdecl)>] 
    extern void initCallback (Callback callback)

    [<DllImport(DllName, EntryPoint = "get_string", CallingConvention = CallingConvention.Cdecl)>] 
    extern void getString (StringBuilder sb)
        
    let Run url = 
        let t = Thread (fun () -> runWebWindow url)
#if Windows
        t.SetApartmentState(ApartmentState.STA)
#endif        

        let callback (text: string) =
//            printf "Das kam vom lieben QT: %s" text
            let t = text
            ()



        printf "Start1"
        let line = Console.ReadLine()


        let sb = StringBuilder 2000
        [1..1000000] |> List.iter (fun i -> 
            getString sb
            let erg = sb.ToString ()
            ()
        ) 
        printf "Ente2"


        let dele = new Callback (callback)
        initCallback dele
        testCallback ()

        printf "Start"
        let line = Console.ReadLine()

        [1..1000000] |> List.iter (fun i -> testCallback ()) 
        printf "Ente"

        t.Start()
        t.Join()
        
