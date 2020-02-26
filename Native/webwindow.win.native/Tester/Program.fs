// Learn more about F# at http://fsharp.org

open System
open System.Runtime.InteropServices

[<Literal>]
let private DllName = "NativeWinWebView"

[<UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet=CharSet.Auto)>]
type Callback = delegate of string -> unit

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
        val mutable callback: Callback
    end

type MenuItemType =  MenuItem = 0 | Checkbox = 1 | Separator = 2

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)>]
type MenuItem = 
    struct 
        val mutable menuItemType: MenuItemType
        val mutable title: string
        val mutable accelerator: string 
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

[<EntryPoint>]
let main argv =
    printfn "Hello World from new F#!"
    let url = @"file://C:\Users\urieg\source\repos\WebWindowNetCore\WebRoot\index.html"
    let url = @"file://D:\Projekte\WebWindowNetCore\WebRoot\index.html"
    //let url = "https://google.de"

    let callback (text: string) =
            printfn "Das kam vom lieben Webview: %s" text
            let t = text
            ()
    let callbackDelegate = Callback callback

    initializeWindow (Configuration(title = "Web Brauser😎😎👌", url = url, iconPath = @"D:\Projekte\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico",
                                debuggingEnabled = true, debuggingPort = 0, organization = "URiegel", application = "TestBrauser", saveWindowSettings = true, fullScreenEnabled = true,
                                callback = callbackDelegate))
    let menu = addMenu "&Datei"
    let cmd = setMenuItem (menu, MenuItem( menuItemType = MenuItemType.MenuItem, title = "&Neu", accelerator = "Strg+N" ))
    let cmd = setMenuItem (menu, MenuItem( menuItemType = MenuItemType.MenuItem, title = "&Öffnen", accelerator = null ))
    let cmd = setMenuItem (menu, MenuItem( menuItemType = MenuItemType.Separator, title = null, accelerator = null ))
    let cmd = setMenuItem (menu, MenuItem( menuItemType = MenuItemType.MenuItem, title = "&Beenden", accelerator = "Alt+F4" ))
    let menu = addMenu "Ansicht"
    let cmd = setMenuItem (menu, MenuItem( menuItemType = MenuItemType.MenuItem, title = "&Versteckte Dateien", accelerator = "Strg+H" ))

    async {
        let rec readLine () = 
            let line = Console.ReadLine ()
            sendToBrowser line
            readLine ()
        readLine ()
    } |> Async.Start

    execute ()
