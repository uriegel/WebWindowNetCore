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

type MenuCmdItem = {
    Title: string
    Accelerator: string option
    Cmd: int
}

type Menu = {
    Title: string
    Items: MenuItem list
}

and MenuItem = Menu of Menu | CmdItem of MenuCmdItem | Separator

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
    
    // To debug on Linux: Chrome: localhost:8888

let execute = NativeMethods.Execute 

let sendToBrowser = NativeMethods.SendToBrowser

let menu: Menu = {
    Title = ""
    Items = [ 
        Menu {
            Title = "&Datei"
            Items = [ 
                CmdItem { Title = "&Umbenennen"; Accelerator = Some "F2"; Cmd = 1 } 
                CmdItem { Title = "&Erweitertes Umbenennen"; Accelerator = Some "Strg+F2"; Cmd = 2 } 
                Separator 
                CmdItem { Title = "&Kopieren"; Accelerator = Some "F5"; Cmd = 3 } 
                CmdItem { Title = "&Verschieben"; Accelerator = Some "F6"; Cmd = 4 } 
                CmdItem { Title = "&Löschen"; Accelerator = Some "Entf"; Cmd = 5 } 
                Separator 
                CmdItem { Title = "&Ordner anlegen"; Accelerator = Some "F7"; Cmd = 6 } 
                Separator 
                CmdItem { Title = "&Eigenschaften"; Accelerator = Some "Alt+Eingabe"; Cmd = 7 } 
                CmdItem { Title = "&Öffnen mit"; Accelerator = Some "Strg+Eingabe"; Cmd = 8 } 
                Separator 
                CmdItem { Title = "&Beenden"; Accelerator = Some "Alt+F4"; Cmd = 9 } 
            ]
        } 
        Menu {
            Title = "&Navigation"
            Items = [ 
                CmdItem { Title = "&Favoriten"; Accelerator = Some "F1"; Cmd = 10 } 
                CmdItem { Title = "&Gleichen Ordner öffnen"; Accelerator = Some "F9"; Cmd = 11 } 
            ]
        }
        Menu {
            Title = "&Selektion"
            Items = [ 
                CmdItem { Title = "&Alles"; Accelerator = Some "Num +"; Cmd = 12 } 
                CmdItem { Title = "Alle &deselektieren"; Accelerator = Some "Num -"; Cmd = 13 } 
                Menu {
                    Title = "&Datei"
                    Items = [ 
                        CmdItem { Title = "&Umbenennen"; Accelerator = Some "F2"; Cmd = 1 } 
                    ]
                }
            ]
        }
        Menu {
            Title = "&Ansicht"
            Items = [ 
                CmdItem { Title = "&Versteckte Dateien"; Accelerator = Some "Strg#H"; Cmd = 14 } 
                CmdItem { Title = "&Aktualisieren"; Accelerator = Some "Strg+R"; Cmd = 15 } 
                Separator 
                CmdItem { Title = "&Vorschau"; Accelerator = Some "F3"; Cmd = 16 } 
                Separator 
                Menu {
                    Title = "&Themen"
                    Items = [ 
                        CmdItem { Title = "&Blau"; Accelerator = None; Cmd = 17 } 
                        CmdItem { Title = "&Hellblau"; Accelerator = None; Cmd = 18 } 
                        CmdItem { Title = "&Dunkel"; Accelerator = None; Cmd = 19 } 
                    ]
                }
                Separator 
                Menu {
                    Title = "&Zoomlevel"
                    Items = [ 
                        CmdItem { Title = "50%"; Accelerator = None; Cmd = 20 } 
                        CmdItem { Title = "75%"; Accelerator = None; Cmd = 21 } 
                        CmdItem { Title = "100%"; Accelerator = None; Cmd = 22 } 
                        CmdItem { Title = "150%"; Accelerator = None; Cmd = 23 } 
                        CmdItem { Title = "200%"; Accelerator = None; Cmd = 24 } 
                        CmdItem { Title = "250%"; Accelerator = None; Cmd = 25 } 
                        CmdItem { Title = "300%"; Accelerator = None; Cmd = 26 } 
                        CmdItem { Title = "350%"; Accelerator = None; Cmd = 27 } 
                        CmdItem { Title = "400%"; Accelerator = None; Cmd = 28 } 
                    ]
                }
                CmdItem { Title = "Voll%bild"; Accelerator = Some "F11"; Cmd = 29 } 
                Separator 
                CmdItem { Title = "&Entwicklungewerkzeuge"; Accelerator = Some "F12"; Cmd = 30 } 
            ]
        }
    ]
}

let affe = menu 

let title = affe.Title
let menu2 = 2

let createMenuItem menu (item: MenuItem)  =
    match item with
    | CmdItem value -> ()
    | Menu value -> ()
    | Separator -> ()

affe.Items |> List.iter (createMenuItem menu2)

 