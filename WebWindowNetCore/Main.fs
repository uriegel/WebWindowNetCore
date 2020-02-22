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
}

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)>]
type private NativeConfiguration = 
    struct 
        val mutable title: string
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
    
    // TODO: Developer Tools
    // TODO: To debug: Chrome: localhost:8888
    // TODO: Save and Resote Window Settings
    // TODO: Icon
    // TODO: Full screen
    // TODO: Menu

let execute = NativeMethods.Execute 