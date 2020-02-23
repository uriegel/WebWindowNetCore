module WebWindowNetCore.WebWindow

open System.Runtime.InteropServices
open System.Threading
open System
open System.Text

[<Literal>]
let private DllName = "NativeWinWebView"

[<UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet=CharSet.Auto)>]
type Callback = delegate of string -> unit

type Configuration = {
    title: string
    url: string
    iconPath: string
    debuggingEnabled: bool
    debuggingPort: int
    organization: string
    application: string
    saveWindowSettings: bool
    fullScreenEnabled: bool
    callback: Callback
}

let defaultConfiguration () = {
    title = "Browser"
    url = "https://www.google.de"
    iconPath = ""
    debuggingEnabled = false
    debuggingPort = 8888
    organization = ""
    application = ""
    saveWindowSettings = false
    fullScreenEnabled = false
    callback = null
}

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)>]
type private NativeConfiguration = 
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
        val mutable callback: Callback
    end

[<AbstractClass>]
type private NativeMethods() =
    [<DllImport(DllName, EntryPoint = "initialize_window", CallingConvention = CallingConvention.Cdecl)>] 
    static extern void nativeInitialize (NativeConfiguration configuration)

    [<DllImport(DllName, EntryPoint = "execute", CallingConvention = CallingConvention.Cdecl)>] 
    static extern int nativeExecute ()

    [<DllImport(DllName, EntryPoint = "send_to_browser", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)>] 
    static extern void nativeSendToBrowser (string text)

    static member Initialize = nativeInitialize
    static member Execute = nativeExecute
    static member SendToBrowser = nativeSendToBrowser

let initialize (configuration: Configuration) =
    let c = NativeConfiguration(
                title = configuration.title, url = configuration.url, iconPath = configuration.iconPath, 
                debuggingEnabled = configuration.debuggingEnabled, debuggingPort = configuration.debuggingPort,
                organization = configuration.organization, application = configuration.application, 
                saveWindowSettings = configuration.saveWindowSettings, fullScreenEnabled = configuration.fullScreenEnabled,
                callback = configuration.callback
            )
    NativeMethods.Initialize c
    
    // TODO Window
    // setCallback in javascript
    
    // TODO: Menu
    // TODO: To debug on Linux: Chrome: localhost:8888

let execute = NativeMethods.Execute 

let sendToBrowser = NativeMethods.SendToBrowser