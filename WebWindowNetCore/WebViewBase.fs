namespace WebWindowNetCore
open Giraffe
open System
open System.Diagnostics
open System.Threading.Tasks
open System.IO
open Microsoft.AspNetCore.Http

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
    // let mutable query: Option<string> = None
    // let mutable getQuery: Option<unit->string> = None
    let mutable debugUrl: Option<string> = None
    let mutable saveBounds = false
    let mutable devTools = false
    let mutable resourceIcon: Option<string> = None
    let mutable resourceScheme = false
    // let mutable withoutNativeTitlebar = false
    // let mutable onWindowStateChanged: Option<WebWindowState->unit> = None
    let mutable onFilesDrop: Option<string->bool->string[]->unit> = None
    let mutable onStarted: Option<WebViewAccess->unit> = None
    let mutable onEventSink: Option<(string*WebViewAccess)->unit> = None
    let mutable canClose: Option<unit->bool> = None
    let mutable requests: Request list = []
    let mutable requestPort = 2222
    let mutable defaultContextMenuDisabled = false
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
    member this.OnFilesDrop(action: string->bool->string[]->unit) = 
        onFilesDrop <- Some action
        this
    member this.OnFilesDrop(action: System.Action<string, bool, string[]>) = 
        onFilesDrop <- Some (fun s b sa -> action.Invoke(s, b, sa))
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
    member this.AddRequest<'input, 'output>(method: string, request: Func<'input, Task<'output>>) =  

        let req () (next : HttpFunc) (ctx : HttpContext) = 
            task {
                let! input = ctx.BindJsonAsync<'input> ()
                let! result = request.Invoke input
                return! json result next ctx
            }

        requests <- requests |> List.append [ { Method = method; Request = req } ] 
        this

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
    let AsTask<'a> (tsk: Task<'a>) =
        task {
            let! t = tsk
            return t :> obj
        }

    let GetInput<'a> (input: Stream) =
        System.Text.Json.JsonSerializer.Deserialize<'a>(input)

    let getScript port windows =

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

        sprintf """
            var webViewEventSinks = new Map()

            var WebView = (() => {
                %s
                %s
                const request = async (method, data) => {
                    const res = await fetch(`http://localhost:%d/${method}`, {
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
                    registerEvents
                }
            })()
        """ devTools onEventsCreated port

// TODO Drag n Drop Windows
// TODO Javascript events from server, perhaps  b e f o r e  javascripts are loaded
// TODO Custom Taskbar Windows
// TODO Custom Taskbar Linux
// TODO Windows client is shrinking with every new start
// TODO CORS cache
// TODO requests url: /requests/{method}
// TODO Stream downloads with Kestrel, icons, jpg, range (mp4, mp3)
// TODO Theme change detection