namespace WebWindowNetCore
open System
open System.Diagnostics
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
#if Linux
open FSharpTools
open GtkDotNet.SafeHandles
#endif
#if Windows
open System.Windows.Forms
#endif
type RequestFun = unit->HttpFunc->HttpContext->Task<option<HttpContext>>

type internal Request = {
    Method: string
    Request: RequestFun
}

[<AbstractClass>]
type WebViewBase() = 
    let mutable appId = "de.uriegel.webwindownetcore"
    let mutable width = 800
    let mutable height = 600
    let mutable title = ""
    let mutable url: Option<string> = None
    let mutable debugUrl: Option<string> = None
    let mutable saveBounds = false
    let mutable devTools = false
    let mutable resourceIcon: Option<string> = None
    let mutable resourceScheme = false
    let mutable withoutNativeTitlebar = false
    let mutable onStarted: Option<WebViewAccess->unit> = None
    let mutable onEventSink: Option<(string*WebViewAccess)->unit> = None
    let mutable canClose: Option<unit->bool> = None
    let mutable requests: Request list = []
    let mutable requestPort = 2222
    let mutable defaultContextMenuDisabled = false
#if Linux
    let mutable titleBar: Option<ApplicationHandle->WindowHandle->ObjectRef<WebViewHandle>->WidgetHandle> = None 
#endif    
#if Windows
    let mutable onFormCreating: Option<Form->unit> = None
    let mutable onHamburger: Option<float->float->unit> = None
    let mutable onFilesDrop: Option<string->bool->string array->unit> = None
#endif
    member internal this.AppIdValue = appId
    member internal this.TitleValue = title
    member internal this.WidthValue = width
    member internal this.HeightValue = height
    member internal this.UrlValue = url
    member internal this.DebugUrlValue = debugUrl
    member internal this.SaveBoundsValue = saveBounds
    member internal this.CanCloseValue = canClose
    member internal this.OnStartedValue = onStarted
    member internal this.OnEventSinkValue = onEventSink
    member internal this.ResourceIconValue = resourceIcon
    member internal this.ResourceSchemeValue = resourceScheme
    member internal this.DevToolsValue = devTools
    member internal this.DefaultContextMenuDisabledValue = defaultContextMenuDisabled
    member internal this.Requests = requests
    member internal this.RequestPortValue = requestPort
    member internal this.WithoutNativeTitlebarValue = withoutNativeTitlebar
#if Linux
    member internal this.TitleBarValue = titleBar    
#endif
#if Windows
    member internal this.OnFormCreatingValue = onFormCreating
    member internal this.OnHamburgerValue = onHamburger
    member internal this.OnFilesDropValue = onFilesDrop
#endif    
    member internal this.GetUrl () = 
        if Debugger.IsAttached then
            this.DebugUrlValue |> Option.defaultValue (this.UrlValue |> Option.defaultValue "")
        else
            this.UrlValue |> Option.defaultValue ""

    member this.AppId(id) =
        appId <- id
        this
    member this.InitialBounds(w, h) =
        width <- w
        height<- h
        this
    member this.Title(t) =
        title <- t                        
        this
    member this.Url(u) =
        url <- Some u                                
        this
    /// <summary>
    /// This url is set to the webview only in debug mode, if HttpBuilder.ResourceWebroot is normally used. 
    /// It is used for React, Vue,... which have their
    /// own web server at debug time, like http://localhost:3000 . If set, it has precedence over 
    /// HttpBuilder.ResourceWebroot
    /// </summary>
    /// <param name="url"></param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.DebugUrl(url) =
        debugUrl <- Some url                                
        this
    member this.SaveBounds() =
        saveBounds <- true
        this
    member this.CanClose(canCloseFunc: Func<bool>) = 
        canClose <- Some canCloseFunc.Invoke
        this
    member this.OnStarted(onStartedFunc: Action<WebViewAccess>) = 
        onStarted <- Some onStartedFunc.Invoke
        this
    member this.OnEventSink(onCreated: Action<string, WebViewAccess>) = 
        onEventSink <- Some onCreated.Invoke
        this
    /// Does not work for Linux
    member this.ResourceIcon(iconName: string) =
        resourceIcon <- Some iconName
        this
    member this.ResourceScheme() =
        resourceScheme <- true
        this
    member this.DevTools() =
        devTools <- true
        this
    member this.DefaultContextMenuDisabled() =
        defaultContextMenuDisabled <- true
        this
    member this.RequestPort(port) =
        requestPort <- port
        this
    /// Only for Windows
    member this.WithoutNativeTitlebar () = 
#if Windows    
        withoutNativeTitlebar <- true
#endif
        this
    member this.AddRequest<'input, 'output>(method: string, request: Func<'input, Task<'output>>) =  

        let req () (next : HttpFunc) (ctx : HttpContext) = 
            task {
                let! input = ctx.BindJsonAsync<'input> ()
                let! result = request.Invoke input
                return! json result next ctx
            }

        requests <- requests |> List.append [ { Method = method; Request = req } ] 
        this
#if Linux
    member this.TitleBar(titleBarCreate: Func<ApplicationHandle, WindowHandle, ObjectRef<WebViewHandle>, WidgetHandle>) =
        titleBar <- Some (fun a w wv -> titleBarCreate.Invoke(a, w, wv))
        this
#endif
#if Windows
    member this.OnFormCreating(formCreateFunc: Action<Form>) =
        onFormCreating <- Some formCreateFunc.Invoke
        this
    member this.OnHamburger(action: Action<float, float>) = 
        onHamburger <- Some (fun rl rt -> action.Invoke(rl, rt))
        this
    /// <summary>
    /// Enable dropping windows files to a drop zone with html identifier 'id'.
    /// When a file/directory or multiple files/directories are dropped to a certain drag zone, this callback is being called from javascript
    /// </summary>
    /// <param name="action">Callback which is called from javascript when dropping files. First parameter is the id of the drag zone, second is if the files are moved (true) or copied (false), the third parameter is the array with the complete filenames</param>
    member this.OnFilesDrop(action: Action<string, bool, string array>) =
        onFilesDrop <- Some (fun id move files -> action.Invoke(id, move, files))
        this
#endif
    abstract member Run: unit->int

and WebViewAccess(executeJavascript: Action<string>, sendEvent: Action<string, obj>) = 
    member this.ExecuteJavascript = executeJavascript
    member this.SendEvent = sendEvent

module ContentType = 
    let get (uri: string) = 
        if uri.EndsWith ".html" then
            "text/html"
        else if uri.EndsWith ".css" then
            "text/css"
        else if uri.EndsWith ".js" then
            "application/javascript"
        else
            "text/text"

module Requests =
    open System.IO
    let AsTask<'a> (tsk: Task<'a>) =
        task {
            let! t = tsk
            return t :> obj
        }

    let GetInput<'a> (input: Stream) =
        System.Text.Json.JsonSerializer.Deserialize<'a>(input)

    let getScript noNativeTitlbar title port windows doFilesDrop =

        let devTools = 
            if windows then
                "const showDevTools = () => callback.ShowDevtools()"
            else
                "const showDevTools = () => fetch('req://showDevTools')"
            
        let onEventsCreated = 
            if windows then
                "const onEventsCreated = id => callback.OnEvents(id)"
            else
                "const onEventsCreated = id => fetch(`req://onEvents/${id}`)"

        let noTitlebarScript = 
            if noNativeTitlbar then 
                sprintf """
                    function WEBVIEWsetMaximized(m) { 
                        const maximize = document.getElementById('$MAXIMIZE$')
                        if (maximize)
                            maximize.hidden = m

                        const restore = document.getElementById('$RESTORE$')
                        if (restore)
                            restore.hidden = !m
                    }

                    (() => {
                        const title = document.getElementById('$TITLE$')
                        if (title)
                            title.innerText = "%s"
                        const close = document.getElementById('$CLOSE$')
                        if (close)
                            close.onclick = () => window.close()
                        const maximize = document.getElementById('$MAXIMIZE$')
                        if (maximize) 
                            maximize.onclick = () => callback.MaximizeWindow()
                        const minimize = document.getElementById('$MINIMIZE$')
                        if (minimize)
                            minimize.onclick = () => callback.MinimizeWindow()
                        const restore = document.getElementById('$RESTORE$')
                        if (restore) {
                            restore.onclick = () => callback.RestoreWindow()
                            restore.hidden = true
                        }
                        const hamburger = document.getElementById('$HAMBURGER$')
                        if (hamburger) 
                            hamburger.onclick = () => callback.OnHamburger(hamburger.offsetLeft / document.body.offsetWidth, (hamburger.offsetTop + hamburger.offsetHeight) / document.body.offsetHeight)
                            
                    })()
                """ title
            else
                ""

        let onFilesDropScript =
            if doFilesDrop then
                @"function dropFiles(id, move, droppedFiles) {
                    chrome.webview.postMessageWithAdditionalObjects({
                        msg: 1,
                        text: id,
                        move
                    }, droppedFiles)
                }"
            else
                "function dropFiles() {}"
        
        sprintf """
            %s

            var webViewEventSinks = new Map()

            var WebView = (() => {
                %s
                %s
                %s
                const request = async (method, data) => {
                    const res = await fetch(`http://localhost:%d/requests/${method}`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify(data)
                    })
                    return await res.json()
                }

                const registerEvents = (id, evt) => {
                    webViewEventSinks.set(id, evt)
                    onEventsCreated(id)
                }

                return {
                    showDevTools,
                    request,
                    registerEvents,
                    dropFiles
                }
            })()

            try {
                if (onWebViewLoaded) 
                    onWebViewLoaded()
            } catch { }
        """ noTitlebarScript devTools onFilesDropScript onEventsCreated port

// TODO Windows: DragStart with files
// TODO CORS cache
// TODO CORS Domains
// TODO Stream downloads with Kestrel, icons, jpg, range (mp4, mp3)
// TODO Theme change detection