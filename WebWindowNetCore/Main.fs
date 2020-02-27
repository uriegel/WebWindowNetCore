module WebWindowNetCore.WebWindow

open System.Runtime.InteropServices
open System.Threading
open System
open System.Text

[<Literal>]
let private DllName = "NativeWinWebView"

[<UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet=CharSet.Auto)>]
type Callback = delegate of string -> unit

[<UnmanagedFunctionPointer(CallingConvention.Cdecl)>]
type MenuCallback = delegate of unit -> unit

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

type MenuItemType =  MenuItem = 0 | Separator = 1 | Checkbox = 2 | Radio = 3

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)>]
type private NativeMenuItem = 
    struct 
        val mutable menuItemType: MenuItemType
        val mutable title: string
        val mutable accelerator: string 
        val mutable onMenu: MenuCallback
        val mutable groupCount: int
        val mutable groupId: int
    end

type MenuCmdItem = {
    Title: string
    Accelerator: string option
    Action: unit -> unit
}

type CheckBoxItem = {
    Title: string
    Accelerator: string option
}

type RadioItem = {
    Title: string
    Accelerator: string option
}

type Menu = {
    Title: string
    Items: MenuItem list
}

and MenuGroup = {
    Items: MenuItem list
}

and MenuItem = Menu of Menu | CmdItem of MenuCmdItem | Separator | Checkbox of CheckBoxItem | Radio of RadioItem | MenuGroup of MenuGroup

[<AbstractClass>]
type private NativeMethods() =
    [<DllImport(DllName, EntryPoint = "initializeWindow", CallingConvention = CallingConvention.Cdecl)>] 
    static extern void nativeInitialize (NativeConfiguration configuration)

    [<DllImport(DllName, EntryPoint = "execute", CallingConvention = CallingConvention.Cdecl)>] 
    static extern int nativeExecute ()

    [<DllImport(DllName, EntryPoint = "sendToBrowser", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)>] 
    static extern void nativeSendToBrowser (string text)

    [<DllImport(DllName, EntryPoint = "addMenu", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)>]     
    static extern IntPtr nativeAddMenu (string title)

    [<DllImport(DllName, EntryPoint = "addSubmenu", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)>] 
    static extern IntPtr nativeAddSubmenu (string title, IntPtr parentMenu)

    [<DllImport(DllName, EntryPoint = "setMenuItem", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)>] 
    static extern int nativeSetMenuItem (IntPtr menu, NativeMenuItem menuItem)

    static member Initialize = nativeInitialize
    static member Execute = nativeExecute
    static member SendToBrowser = nativeSendToBrowser
    static member addMenu = nativeAddMenu
    static member setMenuItem = nativeSetMenuItem
    static member addSubmenu = nativeAddSubmenu

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
    // TODO: setMenuItemSelected on Windows
    // TODO: do the same on Linux
    // TODO: Accelerators on Linux
    // TODO: Accelerators on Windows

let execute = NativeMethods.Execute 

let sendToBrowser = NativeMethods.SendToBrowser



let private dont () = ()
let private dontDelegate = MenuCallback dont






let mutable private delegatesHolder = []
let setMenu (menu: MenuItem list) = 
    let rec setMenu (menu: MenuItem list) (menuHandle: IntPtr) = 
        let createMenuItem (item: MenuItem)  =
            match item with
            | Menu value -> 
                let menuHandle = 
                    if menuHandle = IntPtr.Zero then
                        NativeMethods.addMenu value.Title
                    else
                        NativeMethods.addSubmenu (value.Title, menuHandle)
                setMenu value.Items menuHandle
            | Separator -> 
                NativeMethods.setMenuItem (menuHandle, NativeMenuItem( menuItemType = MenuItemType.Separator, 
                                            title = null, accelerator = null, onMenu = dontDelegate ))|> ignore
            | CmdItem value ->
                let callback = MenuCallback value.Action
                delegatesHolder <- callback :: delegatesHolder
                NativeMethods.setMenuItem (menuHandle, NativeMenuItem( 
                                            menuItemType = MenuItemType.MenuItem,
                                            title = value.Title, 
                                            accelerator = "value.Accelerator",
                                            onMenu = callback)
                                        ) |> ignore
            | Checkbox value ->                                        
                NativeMethods.setMenuItem (menuHandle, NativeMenuItem( 
                                            menuItemType = MenuItemType.Checkbox,
                                            title = value.Title, 
                                            accelerator = "Strg+N",
                                            onMenu = dontDelegate)
                                        ) |> ignore
            | MenuGroup value -> 
                let count = List.length value.Items
                let createRadioItem i item =
                    match item with
                    | Radio value ->
                        NativeMethods.setMenuItem (menuHandle, NativeMenuItem( 
                                                        menuItemType = MenuItemType.Radio,
                                                        title = value.Title, 
                                                        accelerator = "Strg+N",
                                                        onMenu = dontDelegate,
                                                        groupCount = count,
                                                        groupId = i)
                                                    ) |> ignore
                    | _ -> ()
                value.Items |> List.iteri createRadioItem
            | _ -> ()
        menu |> List.iter createMenuItem
    setMenu menu IntPtr.Zero





 