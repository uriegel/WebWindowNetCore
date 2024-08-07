namespace WebWindowNetCore

#if Linux
open GtkDotNet
#endif

type WebView() = 
    let mutable appId = "de.uriegel.webwindownetcore"
    let mutable width = 800
    let mutable height = 600
    let mutable title = ""
    let mutable url: Option<string> = None
    // let mutable query: Option<string> = None
    // let mutable getQuery: Option<unit->string> = None
    // let mutable debugUrl: Option<string> = None
    // let mutable saveBounds = false
    // let mutable devTools = false
    // let mutable resourceIcon: Option<string> = None
    // let mutable withoutNativeTitlebar = false
    // let mutable onWindowStateChanged: Option<WebWindowState->unit> = None
    let mutable onFilesDrop: Option<string->bool->string[]->unit> = None
    // let mutable onStarted: Option<unit->unit> = None
    // let mutable canClose: Option<unit->bool> = None
    // let mutable onScriptAction: Option<int->string->unit> = None
    // let mutable defaultContextMenuEnabled = false

    member this.AppId(id) =
        appId <- id
        this
    member this.Width(w) =
        width <- w        
        this
    member this.Height(h) =
        height <- h                
        this
    member this.Title(t) =
        title <- t                        
        this
    member this.Url(u) =
        url <- Some u                                
        this
    member this.OnFilesDrop(action: string->bool->string[]->unit) = 
        onFilesDrop <- Some action
        this
    member this.OnFilesDrop(action: System.Action<string, bool, string[]>) = 
        onFilesDrop <- Some (fun s b sa -> action.Invoke(s, b, sa))
        this

#if Linux
    member this.Run() =
        Application
            .NewAdwaita(appId)
            .OnActivate(fun app ->
                Application
                    .NewWindow(app) 
                    .Title(title)
                    .DefaultSize(width, height)
                    .Child(WebKit.New()
                        //.Ref())
                        .LoadUri("https://google.de"))
                    .Show()
                    |> ignore)
            .Run(0, 0)
#endif

// TODO
    // public static string GetUri(WebViewSettings settings)
    //     => (Debugger.IsAttached && !string.IsNullOrEmpty(settings.DebugUrl)
    //         ? settings.DebugUrl
    //         : settings.Url != null
    //         ? settings.Url
    //         : $"http://localhost:{settings.HttpSettings?.Port ?? 80}{settings.HttpSettings?.WebrootUrl}/{settings.HttpSettings?.DefaultHtml}")
    //             + (settings.Query ?? settings.GetQuery?.Invoke());
