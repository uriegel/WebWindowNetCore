namespace WebWindowNetCore
#if Windows

open ClrWinApi

module Taskbar =
    let getHandle () = 
        Api.FindWindow("Shell_TrayWnd", "")

    let getHandleOfStartButton () =
        let handleOfDesktop = Api.GetDesktopWindow ()
        Api.FindWindowEx(handleOfDesktop, 0, "button", null)
        
    let show () =
        Api.ShowWindow(getHandle (), ShowWindowFlag.Hide) |> ignore
        Api.ShowWindow(getHandleOfStartButton (), ShowWindowFlag.Normal) |> ignore
 
    let hide () =
        Api.ShowWindow(getHandle (), ShowWindowFlag.Hide) |> ignore
        Api.ShowWindow(getHandleOfStartButton (), ShowWindowFlag.Hide) |> ignore

#endif