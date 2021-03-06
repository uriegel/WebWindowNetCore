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
    Title: string
    Url: string
    IconPath: string
    DebuggingEnabled: bool
    DebuggingPort: int
    Organization: string
    Application: string
    SaveWindowSettings: bool
    FullScreenEnabled: bool
    OnEvent: string -> unit
    DropFiles: string -> unit
}

let defaultConfiguration () = {
    Title = "Browser"
    Url = "https://www.google.de"
    IconPath = ""
    DebuggingEnabled = false
    DebuggingPort = 8888
    Organization = ""
    Application = ""
    SaveWindowSettings = false
    FullScreenEnabled = false
    OnEvent = fun s -> ()
    DropFiles = fun s -> ()
}

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)>]
type private NativeConfiguration = 
    struct 
        val mutable Title: string
        val mutable Url: string
        val mutable IconPath: string
        [<MarshalAs(UnmanagedType.U1)>]
        val mutable DebuggingEnabled: bool
        val mutable DebuggingPort: int
        val mutable Organization: string
        val mutable Application: string
        [<MarshalAs(UnmanagedType.U1)>]
        val mutable SaveWindowSettings: bool
        [<MarshalAs(UnmanagedType.U1)>]
        val mutable FullScreenEnabled: bool
        val mutable Callback: Callback
        val mutable DropFiles: Callback
    end

type MenuItemType =  MenuItem = 0 | Separator = 1 | Checkbox = 2 | Radio = 3

[<StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)>]
type private NativeMenuItem = 
    struct 
        val mutable MenuItemType: MenuItemType
        val mutable Title: string
        val mutable Accelerator: string 
        val mutable OnMenu: MenuCallback
        val mutable OnChecked: MenuCheckedCallback
        val mutable GroupCount: int
        val mutable GroupId: int
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

    [<DllImport(DllName, EntryPoint = "closeWindow", CallingConvention = CallingConvention.Cdecl)>] 
    static extern void nativeCloseWindow ()

    [<DllImport(DllName, EntryPoint = "showDevTools", CallingConvention = CallingConvention.Cdecl)>] 
    static extern void nativeShowDevTools ()
    
    [<DllImport(DllName, EntryPoint = "showFullscreen", CallingConvention = CallingConvention.Cdecl)>] 
    static extern void nativeShowFullscreen (bool fullscreen)

    static member Initialize = nativeInitialize
    static member Execute = nativeExecute
    static member SendToBrowser = nativeSendToBrowser
    static member addMenu = nativeAddMenu
    static member setMenuItem = nativeSetMenuItem
    static member addSubmenu = nativeAddSubmenu
    static member setMenuItemChecked = nativeSetMenuItemChecked
    static member setMenuItemSelected = nativeSetMenuItemSelected
    static member closeWindow = nativeCloseWindow
    static member showDevTools = nativeShowDevTools
    static member showFullscreen = nativeShowFullscreen

let mutable private onEventDelegate = null
let mutable private dropFilesDelegate = null

let initialize (configuration: Configuration) =
    onEventDelegate <- Callback configuration.OnEvent 
    dropFilesDelegate <- Callback configuration.DropFiles
    let c = NativeConfiguration(
                Title = configuration.Title, Url = configuration.Url, IconPath = configuration.IconPath, 
                DebuggingEnabled = configuration.DebuggingEnabled, DebuggingPort = configuration.DebuggingPort,
                Organization = configuration.Organization, Application = configuration.Application, 
                SaveWindowSettings = configuration.SaveWindowSettings, FullScreenEnabled = configuration.FullScreenEnabled,
                Callback = onEventDelegate, DropFiles = dropFilesDelegate
            )
    NativeMethods.Initialize c
    
    // To debug on Linux: Chrome: localhost:8888
    
let execute = NativeMethods.Execute 
let sendToBrowser = NativeMethods.SendToBrowser
let closeWindow = NativeMethods.closeWindow
let showDevTools = NativeMethods.showDevTools
let showFullscreen = NativeMethods.showFullscreen

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
                NativeMethods.setMenuItem (menuHandle, NativeMenuItem( MenuItemType = MenuItemType.Separator, Title = null ))|> ignore
            | CmdItem value ->
                let callback = MenuCallback value.Action
                delegatesHolder <- MenuCallbackType callback :: delegatesHolder
                NativeMethods.setMenuItem (menuHandle, NativeMenuItem( 
                                            MenuItemType = MenuItemType.MenuItem,
                                            Title = value.Title, 
                                            Accelerator = getAccelerator value.Accelerator,
                                            OnMenu = callback)
                                        ) |> ignore
            | Checkbox value ->                                        
                let callback = MenuCheckedCallback value.OnChecked
                delegatesHolder <- MenuCheckedCallbackType callback :: delegatesHolder
                let id = NativeMethods.setMenuItem (
                            menuHandle, NativeMenuItem( 
                                MenuItemType = MenuItemType.Checkbox,
                                Title = value.Title, 
                                Accelerator = getAccelerator value.Accelerator,
                                OnChecked = callback)
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
                                        MenuItemType = MenuItemType.Radio,
                                        Title = radio.Title, 
                                        Accelerator = getAccelerator radio.Accelerator,    
                                        OnMenu = callback,
                                        GroupCount = count,
                                        GroupId = i)
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
                    value setSelected
                    ()
                | None -> ()
            | _ -> ()
        menu |> List.iter createMenuItem
    setMenu menu IntPtr.Zero





 