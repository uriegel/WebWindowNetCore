#if Windows
using System.Text;
using ClrWinApi;
using CsTools;
using CsTools.Extensions;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace WebWindowNetCore.Windows;

class WebViewForm : Form
{
    public WebViewForm(string appDataPath, WebView settings)
    {
        saveBounds = settings.saveBounds;
        appId = settings.appId;
        canClose = settings.canClose;
        request = settings.request;
        devTools = settings.devTools;
        //(this as ComponentModel.ISupportInitialize).BeginInit();
        SuspendLayout();
        webView.AllowExternalDrop = true;
        webView.CreationProperties = null;
        if (settings.backgroundColor.HasValue)
            webView.DefaultBackgroundColor = settings.backgroundColor.Value;
        webView.Dock = DockStyle.Fill;
        webView.TabIndex = 0;
        webView.ZoomFactor = 1;
        Javascript.Initialize(webView);

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
            if (settings.GetUrl().StartsWith("res://"))
                webView.CoreWebView2.AddWebResourceRequestedFilter("res:*", CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.WebResourceRequested += ServeRes;
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = settings.defaultContextMenuDisabled == false;
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            webView.CoreWebView2.WebMessageReceived += WebMessageReceived;
            webView.CoreWebView2.WindowCloseRequested += (s, e) => Close();

            webView.Source = new Uri(settings.GetUrl());

            await webView.ExecuteScriptAsync(WebWindowNetCore.ScriptInjection.Get(true, settings.title)); 
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

    void ShowDevtools() 
    {
        if (devTools)
            webView.CoreWebView2.OpenDevToolsWindow();
    }

    Unit SendTextResponse(int code, string status, string text, CoreWebView2WebResourceResponse response)
        => Unit
            .Value
            .SideEffect(_ =>
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
                response = webView.CoreWebView2.Environment.CreateWebResourceResponse(stream, code,
                        status, $"Content-Type: text/plain");
            });

    Unit SendOk(CoreWebView2WebResourceResponse response)
        => SendTextResponse(200, "OK", "OK", response);

    Unit SendNotFound(CoreWebView2WebResourceResponse response)
        => SendTextResponse(404, "Not Found", "Resource not found", response);

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
                SendNotFound(e.Response);
            }
        }
    }

    void WebMessageReceived(object? _, CoreWebView2WebMessageReceivedEventArgs e) 
    {
        var msg = e.TryGetWebMessageAsString();
        if (request != null && msg?.StartsWith("request") == true)
        {
            var req = Request.Create(msg);
            request(req);
        }
        else if (msg == "showDevTools")
            ShowDevtools();
        else if (msg?.StartsWith("startDragFiles") == true)
        {
            var files = (msg[15..]
                        .Deserialize<string[]>() ?? [])
                        .Select(n => n.Replace("/", "\\"))  
                        .ToArray();
            DoDragDrop(new DataObject(DataFormats.FileDrop, files), DragDropEffects.All);
            WebView.RunJavascript($"WebView.startDragFilesBack()");
        } 
        else if (msg =="droppedFiles" && e.AdditionalObjects != null)
        {
            var files = e.AdditionalObjects
                        .Select(n => (n as CoreWebView2File)?.Path)
                        .ToArray();
            WebView.RunJavascript($"WebView.droppedFilesBack({files.Serialize(Json.Defaults)})");
        }
    }

    void SetDarkMode(bool dark)
        => Invoke(() =>
            {
                Api.DwmSetWindowAttribute(Handle, DwmWindowAttribute.ImmersiveDarkMode, [dark ? 1 : 0], 4);
                BackColor = dark ? Color.Black : Color.White;
            });

    readonly WebView2 webView = new();
    readonly bool devTools;
    readonly Panel panel = new();
    readonly bool saveBounds; 
    readonly string appId;
    readonly Func<bool>? canClose;
    readonly Action<Request>? request;
}


//             this.setMaximized(this.WindowState = FormWindowState.Maximized)


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

//     member this.setMaximized maximized = 
//         panel.Padding <- if maximized then Padding(3, 7, 3, 3) else Padding 0
//         webView.ExecuteScriptAsync(sprintf "WEBVIEWsetMaximized(%s)" <| if maximized then "true" else "false") 
//         |> Async.AwaitTask
//         |> ignore        

//     override this.WndProc(m: byref<Message>) = 
//         if this.DesignMode || not settings.WithoutNativeTitlebarValue then
//             base.WndProc &m
//         else
//             match m.Msg with
//             | WM_NCCALCSIZE -> calcSizeNoTitlebar &m 
//             | _ -> base.WndProc &m 

#endif


    

