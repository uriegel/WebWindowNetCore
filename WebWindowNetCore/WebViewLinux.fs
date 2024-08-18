namespace WebWindowNetCore
#if Linux
open GtkDotNet
open GtkDotNet.Extensions
open GtkDotNet.SafeHandles
open Option
open FSharpTools
open System.Text
open System.Text.Json
open FSharpTools.String

type WebView() = 
    inherit WebViewBase()

    let sendResponse (request: WebkitUriSchemeRequestHandle) (text: string) =
        let bytes = Encoding.UTF8.GetBytes text
        use gbytes = GBytes.New bytes 
        use gstream = MemoryInputStream.New gbytes
        use response = WebKitUriSchemeResponse.New (gstream, bytes.Length)
        let responseHeaders = SoupMessageHeaders.New SoupMessageHeaderType.Response
        responseHeaders.Set([
            MessageHeader("Access-Control-Allow-Origin", "*")
            MessageHeader("Content-Type", "text/plain")
            MessageHeader("Content-Length", sprintf "%d" bytes.Length)
        ]) |> ignore
        response
            .HttpHeaders(responseHeaders)
            .Status(200, "OK") |> ignore
        request.Finish response
    
    override this.Run() =
        let webViewRef = ObjectRef<WebViewHandle>() 

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
                        .Ref(webViewRef)
                        .If(this.ResourceSchemeValue, this.enableResourceScheme)
                        .With(fun w -> this.enableWebViewHost w)
                        .If(this.DevToolsValue,
                            (fun webview -> webview.GetSettings().EnableDeveloperExtras <- true))
                        .If(this.DefaultContextMenuDisabledValue,
                            (fun webview -> webview.DisableContextMenu() |> ignore))
                        .OnLoadChanged(this.onWebViewLoad)
                        .LoadUri(this.GetUrl ()))
                    .With(fun w -> 
                        this.CanCloseValue |> iter (fun canClose -> w.OnClose(fun _ -> canClose() = false) |> ignore))
                    .If(this.SaveBoundsValue, 
                        this.saveBounds)
                    .With(fun w -> 
                                    this.TitleBarValue
                                    |> iter (fun func -> func app w webViewRef |> w.Titlebar |> ignore))
                    .Show()
                    |> ignore)
            .If(this.Requests |> List.length > 0, fun _ -> Server.start this)
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

    member this.onWebViewLoad (webView: WebViewHandle) (load: WebViewLoad) =
        if load = WebViewLoad.Committed then   
            webView.RunJavascript(Requests.getScript false "" this.RequestPortValue false false)
            this.OnStartedValue |> iter (fun f -> f (this.createWebViewAccess(webView)))

    member this.enableResourceScheme (webView: WebViewHandle) =
        let onRequest (request: WebkitUriSchemeRequestHandle) =
            let serveResourceStream (uri: string) (stream: System.IO.Stream) = 
                let bytes = Array.zeroCreate (int stream.Length)
                stream.Read (bytes, 0, bytes.Length) |> ignore
                use gbytes = GBytes.New(bytes)
                use gstream = MemoryInputStream.New(gbytes)
                request.Finish( gstream, bytes.Length, ContentType.get uri) 
            let uri = 
                request.GetUri()
                |> String.substring 6
            uri 
            |> Resources.get 
            |> iter (serveResourceStream uri)

        let context = WebKitWebContext.GetDefault()
        context.RegisterUriScheme("res", onRequest)
        |> ignore

    member this.enableWebViewHost (webView: WebViewHandle) =
        let onRequest (request: WebkitUriSchemeRequestHandle) =
            match request.GetUri () with
            | "req://showDevTools" ->  
                webView.GetInspector().Show()
                sendResponse request "OK"
            |id when id.StartsWith "req://onEvents/" ->  
                this.OnEventSinkValue
                |> Option.iter (fun action -> action(id |> substring 15, (this.createWebViewAccess webView)))
                sendResponse request "OK"
            | _ -> sendResponse request "OK"

        let context = WebKitWebContext.GetDefault()
        context.RegisterUriScheme("req", onRequest)
        |> ignore
        ()

    member this.createWebViewAccess (webView: WebViewHandle) =
        let runJavascript = webView.RunJavascript
        let onEvent (id) (a: obj) = 
            sprintf "webViewEventSinks.get('%s')(%s)" id (JsonSerializer.Serialize(a, TextJson.Default))
            |> webView.RunJavascript
        WebViewAccess (runJavascript, onEvent)
#endif
