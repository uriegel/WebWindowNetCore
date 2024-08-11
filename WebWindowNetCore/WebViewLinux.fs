namespace WebWindowNetCore
#if Linux
open GtkDotNet
open GtkDotNet.Extensions
open GtkDotNet.SafeHandles
open Option
open FSharpTools

type WebView() = 
    inherit WebViewBase()
    
    override this.Run() =
        Application
            .NewAdwaita(this.AppIdValue)
            .OnActivate(fun app ->
                Application
                    .NewWindow(app) 
                    .Title(this.TitleValue)
                    .Choose(
                        this.SaveBoundsValue,
                        this.retrieveBounds,
                        (fun (w: WindowHandle) -> w.DefaultSize(this.WidthValue, this.HeightValue) |> ignore))
                    .Child(WebKit.New()
                        //.Ref())
                        .If(this.DevToolsValue,
                            (fun webview -> webview.GetSettings().EnableDeveloperExtras <- true))
                        .If(this.DefaultContextMenuDisabledValue,
                            (fun webview -> webview.DisableContextMenu() |> ignore))
                        .OnAlert(this.onJavascript)
                        .OnLoadChanged(this.onWebViewLoad)
                        .LoadUri(this.GetUrl ()))
                    .With(fun w -> 
                        this.CanCloseValue |> iter (fun canClose -> w.OnClose(fun _ -> canClose() = false) |> ignore))
                    .If(this.SaveBoundsValue, 
                        this.saveBounds)
                    .Show()
                    |> ignore)
            .Run(0, 0)

    member this.retrieveBounds (w: WindowHandle) =
        let bounds = Bounds.retrieve this.AppIdValue
        w.DefaultSize(bounds.Width |> Option.defaultValue this.WidthValue, bounds.Height |> Option.defaultValue this.HeightValue) |> ignore
        if bounds.IsMaximized then
            w.Maximize()
        |> ignore

    member this.saveBounds (w: WindowHandle) =
        let canClose (_: WindowHandle) =
            { Bounds.retrieve this.AppIdValue with 
                Width = Some (w.GetWidth ())
                Height = Some (w.GetHeight ())
                IsMaximized = (w.IsMaximized ()) }
            |> Bounds.save this.AppIdValue
            false 
        w.OnClose(canClose) 
        |> ignore

    member this.onJavascript (webView: WebViewHandle) (msg: string) =
        let action = TextJson.deserialize<ScriptAction> msg 
        match action.Action with
        | Action.DevTools -> webView.GetInspector().Show()
        |_ ->()

    member this.onWebViewLoad (webView: WebViewHandle) (load: WebViewLoad) =
        if load = WebViewLoad.Committed then   
            if this.DevToolsValue then
                webView.RunJavascript(@"
                    function webViewShowDevTools() {
                        alert(JSON.stringify({action: 1}))
                    }                    
                ")
#endif
