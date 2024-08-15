namespace WebWindowNetCore
#if Windows
open System
open System.Drawing
open System.Runtime.InteropServices
open System.Windows.Forms
open Microsoft.Web.WebView2.Core
open Microsoft.Web.WebView2.WinForms
open FSharpTools
open System.ComponentModel

type WebViewForm(appDataPath: string, settings: WebViewBase) as this = 
    inherit Form()

    let webView = new WebView2()

    do 
        (webView :> ComponentModel.ISupportInitialize).BeginInit()
        this.SuspendLayout();
        webView.AllowExternalDrop <- true
        webView.CreationProperties <- null
        webView.DefaultBackgroundColor <- Color.White
        webView.Location <- Point (0, 0)
        webView.Margin <- Padding 0
        webView.Dock <- DockStyle.Fill
        webView.TabIndex <- 0
        webView.ZoomFactor <- 1

        //OnFormCreation.Invoke(this);

        settings.ResourceIconValue 
        |> Option.iter (fun i -> 
                                Resources.get i
                                |>Option.iter (fun s -> this.Icon <- new Icon (s)))

        this.AutoScaleDimensions <- SizeF(8F, 20F)
        this.AutoScaleMode <- AutoScaleMode.Font

        let bounds = Bounds.retrieve settings.AppIdValue
        if bounds.X.IsSome && bounds.Y.IsSome then
            this.Location <- Point(bounds.X |> Option.defaultValue 0, bounds.Y |> Option.defaultValue 0)
        this.Size <- Size(bounds.Width |> Option.defaultValue settings.WidthValue, bounds.Height |> Option.defaultValue settings.HeightValue)
        this.WindowState <- if bounds.IsMaximized then FormWindowState.Maximized else FormWindowState.Normal

        if settings.SaveBoundsValue then
            this.FormClosing.Add(this.onClosing)

        this.Load.Add(this.onLoad)

        this.Text <- settings.TitleValue
        this.Controls.Add webView

        (webView :> ComponentModel.ISupportInitialize).EndInit ()
        this.ResumeLayout false

        if settings.Requests |> List.length > 0 then
            Server.start settings

        async {
            let! enf = CoreWebView2Environment.CreateAsync(
                        null, 
                        appDataPath, 
                        CoreWebView2EnvironmentOptions(
                            customSchemeRegistrations = ResizeArray<CoreWebView2CustomSchemeRegistration> [ 
                                CoreWebView2CustomSchemeRegistration("res") 
                            ] ))
                        |> Async.AwaitTask
            do! webView.EnsureCoreWebView2Async(enf) |> Async.AwaitTask
            webView.CoreWebView2.AddWebResourceRequestedFilter("res:*", CoreWebView2WebResourceContext.All)
            webView.CoreWebView2.AddHostObjectToScript("Callback", Callback(this))
            webView.CoreWebView2.WebResourceRequested.Add(this.serveRes)
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled <- false
            webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled <- true
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled <- settings.DefaultContextMenuDisabledValue = false
            webView.CoreWebView2.WindowCloseRequested.Add(fun _ -> this.Close())
            webView.CoreWebView2.ContainsFullScreenElementChanged.Add(this.onFullscreen)

            webView.Source <- Uri (settings.GetUrl ())
            webView.ExecuteScriptAsync(@"
                const callback = chrome.webview.hostObjects.Callback
            ") 
                |> Async.AwaitTask
                |> ignore

            webView.ExecuteScriptAsync(Requests.getScript settings.RequestPortValue true) 
                |> Async.AwaitTask
                |> ignore
        } 
        |> Async.StartWithCurrentContext 

    member this.WebView = webView

    member this.MaximizeWindow () = this.WindowState <- FormWindowState.Maximized
    member this.MinimizeWindow() = this.WindowState <- FormWindowState.Minimized
    member this.RestoreWindow() = this.WindowState <- FormWindowState.Normal
    member this.ShowDevtools () = 
        if settings.DevToolsValue then
            webView.CoreWebView2.OpenDevToolsWindow()
    member this.GetWindowState() = (int)this.WindowState

    member this.onLoad (_: EventArgs) =
        let bounds = Bounds.retrieve settings.AppIdValue
        if bounds.X.IsSome && bounds.Y.IsSome 
                && Screen.AllScreens |> Seq.exists (fun s -> s.WorkingArea.IntersectsWith(Rectangle(
                                                                                                            bounds.X |> Option.defaultValue 0, 
                                                                                                            bounds.Y |> Option.defaultValue 0, 
                                                                                                            this.Size.Width, 
                                                                                                            this.Size.Height))) then
            this.Location <- Point(bounds.X |> Option.defaultValue 0 , bounds.Y |> Option.defaultValue 0)
            this.WindowState <- if bounds.IsMaximized then FormWindowState.Maximized else FormWindowState.Normal
        ()
    member this.onClosing (e: FormClosingEventArgs) =
            { Bounds.retrieve settings.AppIdValue with 
                X = if this.WindowState = FormWindowState.Maximized then Some this.RestoreBounds.Location.X else Some this.Location.X
                Y = if this.WindowState = FormWindowState.Maximized then Some this.RestoreBounds.Location.Y else Some this.Location.Y
                Width = if this.WindowState = FormWindowState.Maximized then Some this.RestoreBounds.Size.Width else Some this.Size.Width
                Height = if this.WindowState = FormWindowState.Maximized then Some this.RestoreBounds.Size.Height else Some this.Size.Height
                IsMaximized = this.WindowState = FormWindowState.Maximized }
            |> Bounds.save settings.AppIdValue

    member this.onFullscreen _ =
        if webView.CoreWebView2.ContainsFullScreenElement then
            this.TopMost <- true
            this.FormBorderStyle <- FormBorderStyle.None
            this.WindowState <- FormWindowState.Maximized
            Taskbar.hide ()
        else
            this.TopMost <- false
            this.WindowState <- FormWindowState.Normal
            this.FormBorderStyle <- FormBorderStyle.Sizable
            Taskbar.show ()

    member this.serveRes e = 
        let serveResourceStream (url: string) (stream: System.IO.Stream) = 
            try
                e.Response <- this.WebView.CoreWebView2.Environment.CreateWebResourceResponse(stream, 200, "OK", sprintf "Content-Type: %s" (ContentType.get url))
            with
            | _ ->  e.Response <- this.WebView.CoreWebView2.Environment.CreateWebResourceResponse(null, 404, "Not Found", "")

        let uri = 
            Uri.UnescapeDataString(e.Request.Uri)
            |> String.substring 6

        uri
        |> Resources.get 
        |> Option.iter (serveResourceStream uri)
        ()

    override this.OnClosing(e: CancelEventArgs) = 
        base.OnClosing(e)
        settings.CanCloseValue
        |> Option.iter (fun cc -> e.Cancel <- cc() = false)
        
and [<ComVisible(true)>] Callback(parent: WebViewForm) =

    member this.ShowDevtools() = parent.ShowDevtools()
    member this.MaximizeWindow() = parent.MaximizeWindow()
    member this.MinimizeWindow() = parent.MinimizeWindow()
    member this.RestoreWindow() = parent.RestoreWindow()
    member this.GetWindowState() = parent.GetWindowState()
    
    // public Task DragStart(string fileList)
    //     => JsonSerializer.Deserialize<FileListType>(fileList, JsonWebDefaults)
    //         .Map(flt =>
    //             parent.DragStart(flt!.Path, flt!.FileList));

    // member this.ScriptAction(id: int, msg: string) = parent.ScriptAction(id, msg)
#endif


    

