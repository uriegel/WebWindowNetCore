namespace WebWindowNetCore
#if Windows
open System.Runtime.InteropServices

module Taskbar =
    [<DllImport("user32.dll")>]
    extern int FindWindow(string className, string windowText)
    [<DllImport("user32.dll")>]
    extern int ShowWindow(int hwnd, int command)
    [<DllImport("user32.dll")>]
    extern int FindWindowEx(int parentHandle, int childAfter, string className, int windowTitle)

    [<DllImport("user32.dll")>]
    extern int GetDesktopWindow()

    let SW_HIDE = 0
    let SW_SHOW = 1

    let getHandle () = 
        FindWindow("Shell_TrayWnd", "")

    let getHandleOfStartButton () =
        let handleOfDesktop = GetDesktopWindow()
        FindWindowEx(handleOfDesktop, 0, "button", 0)
        
    let show () =
        ShowWindow(getHandle (), SW_SHOW) |> ignore
        ShowWindow(getHandleOfStartButton (), SW_SHOW) |> ignore
 
    let hide () =
        ShowWindow(getHandle (), SW_HIDE) |> ignore
        ShowWindow(getHandleOfStartButton (), SW_HIDE) |> ignore
#endif