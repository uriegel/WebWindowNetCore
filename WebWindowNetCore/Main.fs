module WebWindowNetCore.WebWindow

open System.Runtime.InteropServices
open System.Threading
open System
open System.Text

[<Literal>]
let private DllName = "NativeWinWebView"

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
    let c = NativeConfiguration(
                title = configuration.title, url = configuration.url, iconPath = configuration.iconPath, 
                debuggingEnabled = configuration.debuggingEnabled, debuggingPort = configuration.debuggingPort,
                organization = configuration.organization, application = configuration.application, 
                saveWindowSettings = configuration.saveWindowSettings, fullScreenEnabled = configuration.fullScreenEnabled
            )
    NativeMethods.Initialize c
    
    // TODO: Save and Restore Window Settings
    // TODO: Icon
    // TODO: Developer Tools
    // TODO: To debug: Chrome: localhost:8888
    // TODO: Full screen
    
    // TODO: Menu

let execute = NativeMethods.Execute 