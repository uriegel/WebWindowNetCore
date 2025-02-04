#if Windows
using ClrWinApi;
using CsTools.Extensions;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace WebWindowNetCore.Windows;

// type WebMsg = {
//     Msg: int
//     Move: bool
//     Text: string 
// }

class WebViewForm : Form
{
    public WebViewForm(string appDataPath, WebView settings)
    {
        saveBounds = settings.saveBounds;
        appId = settings.appId;
        canClose = settings.canClose;
        //(this as ComponentModel.ISupportInitialize).BeginInit();
        SuspendLayout();
        webView.AllowExternalDrop = true;
        webView.CreationProperties = null;
        if (settings.backgroundColor.HasValue)
            webView.DefaultBackgroundColor = settings.backgroundColor.Value;
        webView.Dock = DockStyle.Fill;
        webView.TabIndex = 0;
        webView.ZoomFactor = 1;

//         settings.OnFormCreatingValue
//         |> Option.iter (fun f -> f(this))
        if (settings.resourceIcon != null)
        Icon = new Icon(Resources.Get(settings.resourceIcon)!);
        AutoScaleMode = AutoScaleMode.Font;

        if (settings.saveBounds)
        {
            var bounds = settings.saveBounds 
                ? WebWindowNetCore.Bounds.Retrieve(settings.appId)
                : null;
            Size = new Size(bounds?.Width ?? settings.width, bounds?.Height ?? settings.height);
            WindowState = bounds?.IsMaximized == true? FormWindowState.Maximized : FormWindowState.Normal;
            // if (WindowState == FormWindowState.Maximized)
            //     isMaximized = true;
        }

        if (settings.saveBounds)
            FormClosing += OnClose;
        if (settings.canClose != null)
            FormClosing += OnCanClose;
        HandleCreated += OnHandle;

//         this.Load.Add(this.onLoad)

//         this.QueryContinueDrag.Add(this.onDrop);

//         if settings.WithoutNativeTitlebarValue then
//             this.Resize.Add(this.onResize)

        Text = settings.title;

        panel.Dock = DockStyle.Fill;
        panel.Controls.Add(webView);
        Controls.Add(panel);

        // (webView :> ComponentModel.ISupportInitialize).EndInit ()
        ResumeLayout(false);

        Init();
        async void Init()
        {
            var env = await CoreWebView2Environment.CreateAsync(null, appDataPath, new CoreWebView2EnvironmentOptions(
                            customSchemeRegistrations: [ 
                                new CoreWebView2CustomSchemeRegistration("res")
                            ], additionalBrowserArguments: settings.withoutNativeTitlebar ? "--enable-features=msWebView2EnableDraggableRegions" : ""));
            await webView.EnsureCoreWebView2Async(env);
            //             if settings.GetUrl () |> String.startsWith "res://" || settings.WithoutNativeTitlebarValue then
            if (settings.GetUrl().StartsWith("res://"))
                webView.CoreWebView2.AddWebResourceRequestedFilter("res:*", CoreWebView2WebResourceContext.All);
            //            webView.CoreWebView2.AddHostObjectToScript("Callback", Callback(this))
            webView.CoreWebView2.WebResourceRequested += ServeRes;
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = settings.defaultContextMenuDisabled == false;
            webView.CoreWebView2.WindowCloseRequested += (s, e) => Close();
            //webView.CoreWebView2.ContainsFullScreenElementChanged.Add(this.onFullscreen)
//             if settings.OnFilesDropValue.IsSome then
//                 webView.CoreWebView2.WebMessageReceived.Add(this.OnFilesDropReceived)

            webView.Source = new Uri(settings.GetUrl());
        }
    }

    void OnClose(object? _, FormClosingEventArgs e)
    { 
        var bounds = (saveBounds 
            ? WebWindowNetCore.Bounds.Retrieve(appId)
            : new Bounds(null, null, null, null, false))
            with {
                X = WindowState == FormWindowState.Maximized ? RestoreBounds.Location.X : Location.X,
                Y = WindowState == FormWindowState.Maximized ? RestoreBounds.Location.Y : Location.Y,
                Width = WindowState == FormWindowState.Maximized ? RestoreBounds.Size.Width : Size.Width,
                Height = WindowState == FormWindowState.Maximized ? RestoreBounds.Size.Height : Size.Height,
                IsMaximized = WindowState == FormWindowState.Maximized 
            };
        WebWindowNetCore.Bounds.Save(appId, bounds);
    }

    void OnCanClose(object? _, FormClosingEventArgs e)
    {
        if (canClose?.Invoke() == false)
            e.Cancel = true;
    }

    void OnHandle(object? _, EventArgs e)
    {
        Theme.StartDetection(SetDarkMode);
        SetDarkMode(Theme.IsDark());
    }

    void ServeRes(object? _, CoreWebView2WebResourceRequestedEventArgs e)
    {
        var uri = Uri.UnescapeDataString(e.Request.Uri)[6..].SubstringUntil('?');
        var stream = Resources.Get(uri);

        if (stream != null)
            ServeResourceStream(uri, stream);

        void ServeResourceStream(string url, Stream stream)
        {
            try
            {
                e.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(stream, 200,
                    "OK", $"Content-Type: {url.GetFileExtension()?.ToMimeType() ?? "text/html"}");
            }
            catch
            {
                e.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(null, 404, "Not Found", "");
            }
        }
    }

    void SetDarkMode(bool dark)
        => Invoke(() =>
            {
                Api.DwmSetWindowAttribute(Handle, DwmWindowAttribute.ImmersiveDarkMode, [dark ? 1 : 0], 4);
                BackColor = dark ? Color.Black : Color.White;
            });

    readonly WebView2 webView = new();
    Panel panel = new();
    readonly bool saveBounds; 
    readonly string appId;
    readonly Func<bool>? canClose;
}





//             webView.ExecuteScriptAsync(@"
//                 const callback = chrome.webview.hostObjects.Callback
//             ") 
//                 |> Async.AwaitTask
//                 |> ignore

//             webView.ExecuteScriptAsync(Script.get settings.WithoutNativeTitlebarValue settings.TitleValue settings.RequestPortValue true settings.OnFilesDropValue.IsSome) 
//                 |> Async.AwaitTask
//                 |> ignore

//             this.setMaximized(this.WindowState = FormWindowState.Maximized)

//             settings.OnStartedValue |> Option.iter (fun f -> f (this.createWebViewAccess ()))



//     let WM_NCCALCSIZE = 0x83

//     let mutable isMaximized = false

//     let calcSizeNoTitlebar (m: byref<Message>) =
//         if m.WParam <> 0 then    
//             let nccsp = NcCalcSizeParams.FromIntPtr(m.LParam)
//             nccsp.Rgrc0.Top <- nccsp.Rgrc0.Top + 1
//             nccsp.Rgrc0.Bottom <- nccsp.Rgrc0.Bottom - 5
//             nccsp.Rgrc0.Left <- nccsp.Rgrc0.Left + 5  
//             nccsp.Rgrc0.Right <- nccsp.Rgrc0.Right - 5 
//             System.Runtime.InteropServices.Marshal.StructureToPtr(nccsp, m.LParam, true)
//         else
//             let mutable clnRect = System.Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, typedefof<Rect>) :?> Rect
//             clnRect.Top <- clnRect.Top + 1
//             clnRect.Bottom <- clnRect.Bottom - 5
//             clnRect.Left <- clnRect.Left + 5  
//             clnRect.Right <- clnRect.Right - 5 
//             System.Runtime.InteropServices.Marshal.StructureToPtr(clnRect, m.LParam, true)
//         m.Result <- 0


//     member this.MaximizeWindow () = this.WindowState <- FormWindowState.Maximized
//     member this.MinimizeWindow() = this.WindowState <- FormWindowState.Minimized
//     member this.RestoreWindow() = this.WindowState <- FormWindowState.Normal
//     member this.ShowDevtools () = 
//         if settings.DevToolsValue then
//             webView.CoreWebView2.OpenDevToolsWindow()
//     member this.StartDragFiles (fileList: string) = 
//             this.DoDragDrop(DataObject(DataFormats.FileDrop, (TextJson.deserialize<DragFiles> fileList).Files), DragDropEffects.All)
//     member this.OnEvents(id: string) = 
//         settings.OnEventSinkValue
//         |> Option.iter (fun action -> action(id, (this.createWebViewAccess ())))
//     member this.GetWindowState() = (int)this.WindowState

//     member this.onLoad (_: EventArgs) =
//         let bounds = Bounds.retrieve settings.AppIdValue
//         if bounds.X.IsSome && bounds.Y.IsSome then
//             this.Location <- Point(bounds.X |> Option.defaultValue 0, bounds.Y |> Option.defaultValue 0)
        
//         // let screenBounds = Screen.FromRectangle(this.Bounds).WorkingArea
//         // if screenBounds.Contains(this.Bounds) = false then
//         //     // Adjust the form's location if it is out of bounds
//         //     this.Location <- Point(Math.Max(screenBounds.X, this.Bounds.X), Math.Max(screenBounds.Y, this.Bounds.Y))
//         //     // Ensure the form fits within the screen
//         //     this.Size <- Size(Math.Min(screenBounds.Width, this.Bounds.Width), Math.Min(screenBounds.Height, this.Bounds.Height))

//         if bounds.X.IsSome && bounds.Y.IsSome 
//             && Screen.AllScreens |> Seq.exists (fun s -> s.WorkingArea.IntersectsWith(Rectangle(
//                                                                                                              bounds.X |> Option.defaultValue 0, 
//                                                                                                              bounds.Y |> Option.defaultValue 0, 
//                                                                                                              this.Size.Width, 
//                                                                                                              this.Size.Height))) then
//             this.Location <- Point(bounds.X |> Option.defaultValue 0 , bounds.Y |> Option.defaultValue 0)
//         //     this.WindowState <- if bounds.IsMaximized then FormWindowState.Maximized else FormWindowState.Normal
//         this.Size <- Size(bounds.Width |> Option.defaultValue settings.WidthValue, bounds.Height |> Option.defaultValue settings.HeightValue)


//     member this.onResize (e: EventArgs) =
//         if this.WindowState = FormWindowState.Maximized <> isMaximized then
//             isMaximized <- this.WindowState = FormWindowState.Maximized
//             this.setMaximized isMaximized

//     member this.onFullscreen _ =
//         if webView.CoreWebView2.ContainsFullScreenElement then
//             this.TopMost <- true
//             this.FormBorderStyle <- FormBorderStyle.None
//             this.WindowState <- FormWindowState.Maximized
//             Taskbar.hide ()
//         else
//             this.TopMost <- false
//             this.WindowState <- FormWindowState.Normal
//             this.FormBorderStyle <- FormBorderStyle.Sizable
//             Taskbar.show ()

//     member this.OnFilesDropReceived (e: CoreWebView2WebMessageReceivedEventArgs) =
//         let msg = JsonSerializer.Deserialize<WebMsg>(e.WebMessageAsJson, TextJson.Default)
//         match msg.Msg = 1, settings.OnFilesDropValue with
//         | true, Some func ->
//             let filesDropPathes = 
//                 e.AdditionalObjects
//                 |> Seq.map (fun n -> (n :?> CoreWebView2File).Path)
//                 |> Seq.toArray        
//             func msg.Text msg.Move filesDropPathes
//         | _ -> ()

//     member this.onDrop (e: QueryContinueDragEventArgs) = 
//         if e.Action = DragAction.Drop then
//             webView.ExecuteScriptAsync "WebView.setDroppedEvent(true)"
//             |> Async.AwaitTask
//             |> ignore
//         else if e.Action = DragAction.Cancel then
//             webView.ExecuteScriptAsync "WebView.setDroppedEvent(false)"
//             |> Async.AwaitTask
//             |> ignore
            

//     member this.createWebViewAccess () =
//         let runJavascript str = 
//             this.Invoke(fun () ->
//                 webView.ExecuteScriptAsync str
//                 |> Async.AwaitTask
//                 |> ignore)
//         let onEvent (id) (a: obj) = 
//             try 
//                 this.Invoke(fun () ->
//                     sprintf "webViewEventSinks.get('%s')(%s)" id (JsonSerializer.Serialize(a, TextJson.Default))
//                     |> webView.ExecuteScriptAsync 
//                     |> Async.AwaitTask
//                     |> ignore)
//             with 
//             | _ -> ()
//         WebViewAccess (runJavascript, onEvent)

//     member this.setMaximized maximized = 
//         panel.Padding <- if maximized then Padding(3, 7, 3, 3) else Padding 0
//         webView.ExecuteScriptAsync(sprintf "WEBVIEWsetMaximized(%s)" <| if maximized then "true" else "false") 
//         |> Async.AwaitTask
//         |> ignore        

//     member this.OnHamburger(ratioLeft: float, ratioTop: float) =
//         settings.OnHamburgerValue
//         |> Option.iter (fun f -> f ratioLeft ratioTop)


//     override this.WndProc(m: byref<Message>) = 
//         if this.DesignMode || not settings.WithoutNativeTitlebarValue then
//             base.WndProc &m
//         else
//             match m.Msg with
//             | WM_NCCALCSIZE -> calcSizeNoTitlebar &m 
//             | _ -> base.WndProc &m 


            
// and [<ComVisible(true)>] Callback(parent: WebViewForm) =

//     member this.ShowDevtools() = parent.ShowDevtools()
//     member this.StartDragFiles(files: string) = parent.StartDragFiles(files)
//     member this.OnEvents(id: string) = parent.OnEvents(id)
//     member this.MaximizeWindow() = parent.MaximizeWindow()
//     member this.MinimizeWindow() = parent.MinimizeWindow()
//     member this.RestoreWindow() = parent.RestoreWindow()
//     member this.OnHamburger(ratioLeft: float, ratioTop: float) = parent.OnHamburger(ratioLeft, ratioTop)
//     // public Task DragStart(string fileList)
//     //     => JsonSerializer.Deserialize<FileListType>(fileList, JsonWebDefaults)
//     //         .Map(flt =>
//     //             parent.DragStart(flt!.Path, flt!.FileList));

//     // member this.ScriptAction(id: int, msg: string) = parent.ScriptAction(id, msg)
#endif


    

