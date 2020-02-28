module WebWindowNetCore.WebWindow

open System.Runtime.InteropServices
open System.Threading
open System
open System.Text

[<Literal>]
let private DllName = "NativeWinWebView"

[<UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet=CharSet.Auto)>]
type private Callback = delegate of string -> unit

[<UnmanagedFunctionPointer(CallingConvention.Cdecl)>]
type private MenuCallback = delegate of unit -> unit

[<UnmanagedFunctionPointer(CallingConvention.Cdecl)>]
type private MenuCheckedCallback = delegate of bool -> unit

type private MenuCallbacks = MenuCallbackType of MenuCallback | MenuCheckedCallbackType of MenuCheckedCallback

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
    onEvent: string -> unit
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
    onEvent = fun s -> ()
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
        val mutable onChecked: MenuCheckedCallback
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
    OnChecked: bool -> unit
    SetCheckedFunction: ((bool -> unit) -> unit) option
}

type RadioItem = {
    Title: string
    Accelerator: string option
    Key: obj
}

type Menu = {
    Title: string
    Items: MenuItem list
}

and MenuGroup = {
    Items: MenuItem list
    OnSelected: obj -> unit
    SetSelected: ((obj -> unit) -> unit) option
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

    [<DllImport(DllName, EntryPoint = "setMenuItemChecked", CallingConvention = CallingConvention.Cdecl)>] 
    static extern void nativeSetMenuItemChecked (int cmdId, [<MarshalAs(UnmanagedType.I1)>] bool isChecked)

    [<DllImport(DllName, EntryPoint = "setMenuItemSelected", CallingConvention = CallingConvention.Cdecl)>] 
    static extern void nativeSetMenuItemSelected (int cmdId, int groupCount, int id)

    static member Initialize = nativeInitialize
    static member Execute = nativeExecute
    static member SendToBrowser = nativeSendToBrowser
    static member addMenu = nativeAddMenu
    static member setMenuItem = nativeSetMenuItem
    static member addSubmenu = nativeAddSubmenu
    static member setMenuItemChecked = nativeSetMenuItemChecked
    static member setMenuItemSelected = nativeSetMenuItemSelected

let mutable private onEventDelegate = null

let initialize (configuration: Configuration) =
    onEventDelegate <- Callback configuration.onEvent 
    let c = NativeConfiguration(
                title = configuration.title, url = configuration.url, iconPath = configuration.iconPath, 
                debuggingEnabled = configuration.debuggingEnabled, debuggingPort = configuration.debuggingPort,
                organization = configuration.organization, application = configuration.application, 
                saveWindowSettings = configuration.saveWindowSettings, fullScreenEnabled = configuration.fullScreenEnabled,
                callback = onEventDelegate
            )
    NativeMethods.Initialize c
    
    // To debug on Linux: Chrome: localhost:8888
    // TODO: do the same on Linux
    // TODO: Accelerators on Linux

let execute = NativeMethods.Execute 

let sendToBrowser = NativeMethods.SendToBrowser

let mutable private delegatesHolder: MenuCallbacks list = []

let setMenu (menu: MenuItem list) = 
    let getAccelerator acc =
        match acc with
        | Some value -> value
        | None -> null

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
                NativeMethods.setMenuItem (menuHandle, NativeMenuItem( menuItemType = MenuItemType.Separator, title = null ))|> ignore
            | CmdItem value ->
                let callback = MenuCallback value.Action
                delegatesHolder <- MenuCallbackType callback :: delegatesHolder
                NativeMethods.setMenuItem (menuHandle, NativeMenuItem( 
                                            menuItemType = MenuItemType.MenuItem,
                                            title = value.Title, 
                                            accelerator = getAccelerator value.Accelerator,
                                            onMenu = callback)
                                        ) |> ignore
            | Checkbox value ->                                        
                let callback = MenuCheckedCallback value.OnChecked
                delegatesHolder <- MenuCheckedCallbackType callback :: delegatesHolder
                let id = NativeMethods.setMenuItem (
                            menuHandle, NativeMenuItem( 
                                menuItemType = MenuItemType.Checkbox,
                                title = value.Title, 
                                accelerator = getAccelerator value.Accelerator,
                                onChecked = callback)
                            ) 
                match value.SetCheckedFunction with
                | Some value -> 
                    let setChecked isChecked = NativeMethods.setMenuItemChecked (id, isChecked)
                    value setChecked
                | None -> ()
            | MenuGroup value -> 
                let count = List.length value.Items
                let mutable cmd = -1
                let createRadioItem i item =
                    match item with
                    | Radio radio ->
                        let onSelected () = value.OnSelected radio.Key
                        let callback = MenuCallback onSelected
                        delegatesHolder <- MenuCallbackType callback :: delegatesHolder
                        let id = NativeMethods.setMenuItem (menuHandle, 
                                    NativeMenuItem( 
                                        menuItemType = MenuItemType.Radio,
                                        title = radio.Title, 
                                        accelerator = getAccelerator radio.Accelerator,    
                                        onMenu = callback,
                                        groupCount = count,
                                        groupId = i)
                                    ) 
                        if cmd = -1 then cmd <- id
                        radio.Key
                    | _ -> null
                let keys = 
                    value.Items 
                    |> List.mapi createRadioItem

                match value.SetSelected with
                | Some value -> 
                    let setSelected key = 
                        let id = keys |> List.findIndex (fun n -> n = key)
                        NativeMethods.setMenuItemSelected (cmd, count, id)
                        ()
                    value setSelected
                    ()
                | None -> ()
            | _ -> ()
        menu |> List.iter createMenuItem
    setMenu menu IntPtr.Zero





 