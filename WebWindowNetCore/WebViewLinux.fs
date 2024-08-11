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
                        .If(this.ResourceSchemeValue, this.enableResourceScheme)
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
    member this.enableResourceScheme (webView: WebViewHandle) =
        let onRequest request (_: nativeint) =
            let serveResourceStream (uri: string) (stream: System.IO.Stream) = 
                let getContentType (uri: string) = 
                    if uri.EndsWith ".html" then
                        "text/html"
                    else if uri.EndsWith ".css" then
                        "text/css"
                    else if uri.EndsWith ".js" then
                        "application/javascript"
                    else
                        "text/text"
                let bytes = Array.zeroCreate (int stream.Length)
                stream.Read (bytes, 0, bytes.Length) |> ignore
                use gbytes = GBytes.New(bytes)
                use gstream = MemoryInputStream.New(gbytes)
                WebKitUriSchemeRequest.Finish(request, gstream, bytes.Length, getContentType uri) 
                
                
                
                
                
                |> ignore




                ()
            let uri = 
                WebKitUriSchemeRequest.GetUri(request)
                |> String.substring 6
            uri 
            |> Resources.get 
            |> Option.iter (serveResourceStream uri)

        let context = WebKitWebContext.GetDefault()
        context.RegisterUriScheme("res", onRequest)
        |> ignore
#endif
