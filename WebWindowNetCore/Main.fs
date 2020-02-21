module WebWindowNetCore.WebWindow

open System.Runtime.InteropServices
open System.Threading
open System
open System.Text

#if Linux 
[<Literal>]
let private DllName = "libwebwindowlinuxnative.so.1.0.0"
#else
[<Literal>]
let private DllName = "NativeWinWebView.dll"
#endif            

// #if DEBUG 
// let private debug_mode = true
// #else
// let private debug_mode = false
// #endif

type Configuration = {
    title: string
    url: string
}

[<StructLayout(LayoutKind.Sequential)>]
type private NativeConfiguration = 
    struct 
#if Linux 
        [<MarshalAs(UnmanagedType.LPUTF8Str)>]
#else   
        [<MarshalAs(UnmanagedType.LPWStr)>]     
#endif
        val mutable title: string
#if Linux 
        [<MarshalAs(UnmanagedType.LPUTF8Str)>]
#else   
        [<MarshalAs(UnmanagedType.LPWStr)>]     
#endif
        val mutable url: string
    end

[<AbstractClass>]
type private NativeMethods() =
    [<DllImport(DllName, EntryPoint = "initialize_window", CallingConvention = CallingConvention.Cdecl)>] 
    static extern void nativeInitialize (NativeConfiguration configuration)

    [<DllImport(DllName, EntryPoint = "execute", CallingConvention = CallingConvention.Cdecl)>] 
    static extern int nativeExecute ()

    static member Initialize = nativeInitialize
    static member Execute = nativeExecute


let initialize (configuration: Configuration) =
    let c = NativeConfiguration(title = configuration.title, url = configuration.url)
    NativeMethods.Initialize c
    
    // TODO: To debug: Chrome: localhost:8888

let execute = NativeMethods.Execute 