// Learn more about F# at http://fsharp.org

open System
open System.Runtime.InteropServices

[<Literal>]
let private DllName = "NativeWinWebView"

[<UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet=CharSet.Auto)>]
type EventCallback = delegate of string -> unit

[<UnmanagedFunctionPointer(CallingConvention.Cdecl)>]
type MenuCallback = delegate of unit -> unit

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)>]
type Configuration = 
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
        val mutable callback: EventCallback
        val mutable dropFilesCallback: EventCallback
    end

type MenuItemType =  MenuItem = 0 | Separator = 1 | Checkbox = 2 | Radio = 3

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)>]
type MenuItem = 
    struct 
        val mutable menuItemType: MenuItemType
        val mutable title: string
        val mutable accelerator: string 
        val mutable onMenu: MenuCallback
        val mutable groupCount: int
        val mutable groupId: int
    end

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern void initializeWindow (Configuration configuration)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern int execute ()

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)>] 
extern void sendToBrowser (string text)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)>]     
extern IntPtr addMenu (string title)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)>] 
extern IntPtr addSubmenu (string title, IntPtr parentMenu)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)>] 
extern int setMenuItem (IntPtr menu, MenuItem menuItem)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern [<MarshalAs(UnmanagedType.Bool)>] bool getMenuItemChecked (int cmdId)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern void setMenuItemChecked (int cmdId, [<MarshalAs(UnmanagedType.I1)>] bool isChecked)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>] 
extern void setMenuItemSelected (int cmdId, int groupCount, int id)

[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>]
extern void showDevTools ()
 
[<DllImport(DllName, CallingConvention = CallingConvention.Cdecl)>]
extern void showFullscreen (bool show)

[<DllImport("user32.dll", SetLastError = true)>]
extern bool SetProcessDpiAwarenessContext(int dpiFlag)

[<EntryPoint>]
[<STAThread>]
let main argv =

    let affe = SetProcessDpiAwarenessContext 18
    let errrr = Marshal.GetLastWin32Error ()
    printfn "Hello World from new F#!"
    //let url = @"file://C:\Users\urieg\source\repos\WebWindowNetCore\WebRoot\index.html"
    //let url = @"file://D:\Projekte\WebWindowNetCore\WebRoot\index.html"
    let url = "https://google.de"
    //let url = "http://localhost:8080"

    let callback (text: string) =
            printfn "Das kam vom lieben Webview: %s" text
            let t = text
            ()

    let dropFiles (text: string) =
        let files = text
        ()

    let callbackDelegate = EventCallback callback
    let dropFilesDelegate = EventCallback dropFiles

    initializeWindow (Configuration(title = "Web Brauser😎😎👌", url = url, iconPath = @"D:\Projekte\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico",
                                debuggingEnabled = true, debuggingPort = 0, organization = "URiegel", application = "TestBrauser", saveWindowSettings = true, fullScreenEnabled = true,
                                callback = callbackDelegate, dropFilesCallback = dropFilesDelegate))

    let onNew () = printfn "onNew"
    let onOpen () = printfn "onOpen"
    let onDev () = 
        let th = System.Threading.Thread(fun () -> showDevTools ())
        th.Start()
    let onExit () = printfn "onExit"
    let onShowFullscreen () = showFullscreen true
    let onHidden () = printfn "onHidden" 
    let onRot () = printfn "onRot" 
    let onBlau () = printfn "onBlau" 
    let onDunkel () = printfn "onDunkel" 
    let dont () = ()
    let dontDelegate = MenuCallback dont
    let onNewDelegate = MenuCallback onNew
    let onOpenDelegate = MenuCallback onOpen
    let onExitDelegate = MenuCallback onExit
    let onDevDelegate = MenuCallback onDev
    let onShowFullscreenDelegate = MenuCallback onShowFullscreen
    let onHiddenDelegate = MenuCallback onHidden
    let onRotDelegate = MenuCallback onRot
    let onBlauDelegate = MenuCallback onBlau
    let onDunkelDelegate = MenuCallback onDunkel
    
    //let menu = addMenu "&Datei"
    //setMenuItem (menu, MenuItem( menuItemType = MenuItemType.MenuItem, title = "&Neu", accelerator = "Strg+N", onMenu = onNewDelegate)) |> ignore
    //setMenuItem (menu, MenuItem( menuItemType = MenuItemType.MenuItem, title = "&Öffnen", accelerator = "F5", onMenu = onOpenDelegate ))|> ignore
    //setMenuItem (menu, MenuItem( menuItemType = MenuItemType.Separator, title = null, accelerator = null, onMenu = dontDelegate ))|> ignore
    //setMenuItem (menu, MenuItem( menuItemType = MenuItemType.MenuItem, title = "&Beenden", accelerator = "Alt+F4", onMenu = onExitDelegate ))|> ignore
    //let menu = addMenu "Ansicht"
    //setMenuItem (menu, MenuItem( menuItemType = MenuItemType.MenuItem, title = "&Deff-Tools", accelerator = "Strg+F12", onMenu = onDevDelegate ))|> ignore
    //setMenuItem (menu, MenuItem( menuItemType = MenuItemType.MenuItem, title = "&Vollbild", accelerator = "F11", onMenu = onShowFullscreenDelegate ))|> ignore
    //let hiddenId = setMenuItem (menu, MenuItem( menuItemType = MenuItemType.Checkbox, title = "&Versteckte Dateien", accelerator = "Strg+H", onMenu = onHiddenDelegate ))
    //setMenuItemChecked(hiddenId, true)
    //let submenu = addSubmenu ("&Themen", menu)
    //let themeId = setMenuItem (submenu, MenuItem( menuItemType = MenuItemType.Radio, title = "&Rot", accelerator = null, onMenu = onRotDelegate, groupCount = 3, groupId = 0 ))
    //setMenuItem (submenu, MenuItem( menuItemType = MenuItemType.Radio, title = "&Blau", accelerator = null, onMenu = onBlauDelegate, groupCount = 3, groupId = 1 ))|> ignore
    //setMenuItem (submenu, MenuItem( menuItemType = MenuItemType.Radio, title = "&Dunkel", accelerator = null, onMenu = onDunkelDelegate, groupCount = 3, groupId = 2 ))|> ignore

    //setMenuItemSelected (themeId, 3, 1)
    
    async {
        let rec readLine () = 
            let line = Console.ReadLine ()
            sendToBrowser line
            readLine ()
        readLine ()
    } |> Async.Start

    execute ()
