namespace WebWindowNetCore
open System
open System.Diagnostics
open System.Drawing
open System.IO
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
#if Linux
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

type DragFiles = {
    Files: string array
}

[<AbstractClass>]
type WebViewBase() = 
    let mutable appId = "de.uriegel.webwindownetcore"
    let mutable width = 800
    let mutable height = 600
    let mutable title = ""
    let mutable backgroundColor: Color option = None
    let mutable url: Option<string> = None
    let mutable debugUrl: Option<string> = None
    let mutable saveBounds = false
    let mutable devTools = false
    let mutable resourceIcon: Option<string> = None
    let mutable withoutNativeTitlebar = false
    let mutable onStarted: Option<WebViewAccess->unit> = None
    let mutable onEventSink: Option<(string*WebViewAccess)->unit> = None
    let mutable canClose: Option<unit->bool> = None
    let mutable requests: Request list = []
    let mutable requestsDelegates: Action<IApplicationBuilder> array = [||]
    let mutable rawRequests: (HttpFunc->HttpContext->HttpFuncResult) list = [] 
    let mutable requestPort = 2222
    let mutable defaultContextMenuDisabled = false
    let mutable corsDomains: string array = [||]
    let mutable corsCache = TimeSpan.FromSeconds 5
    let mutable resourceFromHttp = false
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
    member internal this.BackgroundColorValue = backgroundColor
    member internal this.WidthValue = width
    member internal this.HeightValue = height
    member internal this.UrlValue = url
    member internal this.DebugUrlValue = debugUrl
    member internal this.SaveBoundsValue = saveBounds
    member internal this.CanCloseValue = canClose
    member internal this.OnStartedValue = onStarted
    member internal this.OnEventSinkValue = onEventSink
    member internal this.ResourceIconValue = resourceIcon
    member internal this.DevToolsValue = devTools
    member internal this.DefaultContextMenuDisabledValue = defaultContextMenuDisabled
    member internal this.RequestsValue = requests
    member internal this.RawRequestsValue = rawRequests
    member internal this.RequestsDelegatesValue = requestsDelegates
    member internal this.RequestPortValue = requestPort
    member internal this.WithoutNativeTitlebarValue = withoutNativeTitlebar
    member internal this.CorsDomainsValue = corsDomains
    member internal this.CorsCacheValue = corsCache
    member internal this.ResourceFromHttpValue = resourceFromHttp
#if Linux
    member internal this.TitleBarValue = titleBar    
#endif
#if Windows
    member internal this.OnFormCreatingValue = onFormCreating
    member internal this.OnHamburgerValue = onHamburger
    member internal this.OnFilesDropValue = onFilesDrop
#endif    
    member internal this.GetUrl () = 
        let getUrl () = 
            if this.ResourceFromHttpValue then
                sprintf "http://localhost:%d/webroot/index.html" requestPort 
            else
                this.UrlValue |> Option.defaultValue ""

        if Debugger.IsAttached then
            this.DebugUrlValue |> Option.defaultValue (getUrl ())
        else
            getUrl ()


    /// <summary>
    /// The AppId is necessary for a webview app on Linux, it is the AppId for a GtkApplication. 
    /// It is a reverse domain name, like "de.uriegel.webapp"
    /// </summary>
    /// <param name="id">The AppId</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.AppId(id) =
        appId <- id
        this
    
    /// <summary>
    /// With the help of this property you can initialize the size of the window with custom values.
    /// In combination with "SaveBounds()" this is the initial width and heigth of the window at first start,
    /// otherwise the window is always starting with these values.
    /// </summary>
    /// <param name="w">The initial width of the window</param>
    /// <param name="h">The initial height of the window</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.InitialBounds(w, h) =
        width <- w
        height<- h
        this

    /// <summary>
    /// The window title is set by this method.
    /// </summary>
    /// <param name="t">Window title</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.Title(t) =
        title <- t                        
        this

    /// <summary>
    /// Setting the background color of the web view. Normally the html page has its own background color, 
    /// but when starting and before the html page is loaded, this property is active and this color is shown. 
    /// To prevent flickering when starting the app, adapt the BackgroundColor to the http page's value.
    /// </summary>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.BackgroundColor(color: Color) = 
        backgroundColor <- Some color
        this
    
    /// <summary>
    /// Here you set the url of the web view. You can use "http(s)://" scheme, "file://" scheme, and custom resource scheme "res://". This value is 
    /// not used, when you set "DebugUrl" and a debugger is attached
    /// </summary>
    /// <param name="u">The webview's url</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.Url(u) =
        url <- Some u                                
        this

    /// <summary>
    /// This url is set to the webview only when a debugger is attached.  
    /// It is used for React, Vue,... which have their
    /// own web server at debug time, like http://localhost:3000 . If set, it has precedence over 
    /// "Url"
    /// </summary>
    /// <param name="url">The url for a web app being debugged</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.DebugUrl(url) =
        debugUrl <- Some url                                
        this

    /// <summary>
    /// When you call "SaveBounds", then windows location and width and height and normal/maximized state is saved on close. 
    /// After restarting the app the webview is displayed at these settings again.
    /// The "AppId" is used to create a path, where these settings are saved.
    /// </summary>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.SaveBounds() =
        saveBounds <- true
        this

    /// <summary>
    /// Here you can set a callback function which is called when the window is about to close. 
    /// In the callback you can prevent the close request by returning false.
    /// </summary>
    /// <param name="canCloseFunc">Callback funciton called when the window should be closed. Return "true" to close the window, "false" to prevent</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.CanClose(canCloseFunc: Func<bool>) = 
        canClose <- Some canCloseFunc.Invoke
        this

    /// <summary>
    /// Callback which is called when the web view is loaded.
    /// </summary>
    /// <param name="onStartedFunc">Callback function called when the webview is loaded. Parameter is this WebViewBuilder</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.OnStarted(onStartedFunc: Action<WebViewAccess>) = 
        onStarted <- Some onStartedFunc.Invoke
        this

    /// <summary>
    /// Callback which is called when an event handler is registered in javascript
    /// </summary>
    /// <param name="onCreated">Callback when the event handler is ready</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.OnEventSink(onCreated: Action<string, WebViewAccess>) = 
        onEventSink <- Some onCreated.Invoke
        this
    
    /// <summary>
    /// Used to display a windows icon from C# resource. It is only working on Windows.
    /// </summary>
    /// <param name="iconName">Logical name of the resource icon</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.ResourceIcon(iconName: string) =
        resourceIcon <- Some iconName
        this

    /// <summary>
    /// Used to enable (not to show) the developer tools. If not called, it is not possible to open these tools.
    /// The developer tools can be shown by default context menu or by calling the javascript method WebView.showDevtools()
    /// </summary>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.DevTools() =
        devTools <- true
        this

    /// <summary>
    /// When called the web view's default context menu is not being displayed when you right click the mouse.
    /// </summary>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.DefaultContextMenuDisabled() =
        defaultContextMenuDisabled <- true
        this

    /// <summary>
    /// Used to change the port of the included HTTP Kestrel server from 2222 to one of your choice
    /// </summary>
    /// <param name="port">TCP port</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.RequestPort(port) =
        requestPort <- port
        this
    
    /// Only for Windows
    member this.WithoutNativeTitlebar () = 
#if Windows    
        withoutNativeTitlebar <- true
#endif
        this
    
    /// <summary>
    /// With the help of the included HTTP (Kestrel) server your web site can communicate with the app. You can add json post requests
    /// </summary>
    /// <param name="method">A name for the request method</param>
    /// <param name="request">A function getting generic input as the input data and returning a Task of generic output data</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.AddRequest<'input, 'output>(method: string, request: Func<'input, Task<'output>>) =  

        let req () (next : HttpFunc) (ctx : HttpContext) = 
            task {
                let! input = ctx.BindJsonAsync<'input> ()
                let! result = request.Invoke input
                return! json result next ctx
            }

        requests <- requests |> List.append [ { Method = method; Request = req } ] 
        this

    /// <summary>
    /// Setting low level Kestrel requests, e.g. for downloading file streams or images
    /// </summary>
    /// <param name="requests">Array of builders for creating requests with the IApplicationBuilder</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.RequestsDelegates(requests: Action<IApplicationBuilder> array) = 
        requestsDelegates <- requests
        this        

    /// <summary>
    /// Setting low level Kestrel requests, e.g. for downloading file streams or images, with the help of Giraffe (for F#)
    /// </summary>
    /// <param name="requests">List of Kestrel routes</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.Requests(requests: (HttpFunc->HttpContext->HttpFuncResult) list) = 
        rawRequests <- requests 
        this        

    /// <summary>
    /// Setting the enabled domains for CORS requests
    /// </summary>
    /// <param name="domains">Array of CORS domains</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.CorsDomains (domains: string[]) = 
        corsDomains <- domains
        this

    /// <summary>
    /// Setting the duration of the CORS cache for preventing preflights
    /// </summary>
    /// <param name="duration">Duration of the CORS cache</param>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.CorsCache (duration: TimeSpan) = 
        corsCache <- duration
        this

    /// <summary>
    /// Host web site from resource via the included Kestrel HTTP-Server
    /// </summary>
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.ResourceFromHttp () =
        resourceFromHttp <- true
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
    /// <returns>WebView for chaining (fluent Syntax)</returns>
    member this.OnFilesDrop(action: Action<string, bool, string array>) =
        onFilesDrop <- Some (fun id move files -> action.Invoke(id, move, files))
        this
#endif

    /// <summary>
    /// Runs the built app and displays the web view
    /// </summary>
    /// <returns>Exit code</returns>
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
        else if uri.EndsWith ".svg" then
            "image/svg+xml"
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


