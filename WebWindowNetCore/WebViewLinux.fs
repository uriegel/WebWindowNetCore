namespace WebWindowNetCore
#if Linux
open GtkDotNet
open GtkDotNet.Extensions
open GtkDotNet.SafeHandles
open Option
open FSharpTools
open System.Text

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

    member this.onWebViewLoad (webView: WebViewHandle) (load: WebViewLoad) =
        if load = WebViewLoad.Committed then   
            webView.RunJavascript(@"
                var WebView = (() => {
                    const showDevTools = () => fetch('req://showDevTools')
                    
                    const request = (method, data) => new Promise(res => {
                        (async () => {
                            const res = await fetch(`req://${method}`, {
                                method: 'POST',
                                headers: { 'Content-Type': 'application/json' },
                                body: JSON.stringify(data)
                            })
                            const text = await res.text()
                            console.log('reqId', text)
                        })()
                    })

                    return {
                        showDevTools,
                        request
                    }
                })()
            ")

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
            let uri = request.GetUri ()
            if uri = "req://showDevTools" then
                webView.GetInspector().Show()
            else
                let reqId = RequestId.get ()
                use stream = request.GetHttpBody ()
                let bytes = Encoding.UTF8.GetBytes(sprintf "%d" reqId)
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
                // let test = Json.JsonSerializer.Deserialize<Test>(stream, defaults)
                // let t = test
                ()

        let context = WebKitWebContext.GetDefault()
        context.RegisterUriScheme("req", onRequest)
        |> ignore

#endif
