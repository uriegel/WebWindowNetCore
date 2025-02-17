#if Windows
using System.Runtime.InteropServices;
using System.Text;
using ClrWinApi;
using CsTools;
using CsTools.Extensions;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace WebWindowNetCore.Windows;

class WebViewForm : Form
{
    public WebView2 WebView { get; } = new();

    public WebViewForm(string appDataPath, WebView settings)
    {
        saveBounds = settings.saveBounds;
        appId = settings.appId;
        canClose = settings.canClose;
        devTools = settings.devTools;
        width = settings.width;
        height = settings.height;
        withoutNativeTitlebar = settings.withoutNativeTitlebar;
        settings.Initialize(this);
        SuspendLayout();
        WebView.AllowExternalDrop = true;
        WebView.CreationProperties = null;
        if (settings.backgroundColor.HasValue)
            WebView.DefaultBackgroundColor = settings.backgroundColor.Value;
        WebView.Dock = DockStyle.Fill;
        WebView.TabIndex = 0;
        WebView.ZoomFactor = 1;
        settings.onformCreate?.Invoke(this);

        if (settings.resourceIcon != null)
            Icon = new Icon(Resources.Get(settings.resourceIcon)!);
        AutoScaleMode = AutoScaleMode.Font;

        if (settings.saveBounds)
        {
            var bounds = settings.saveBounds
                ? WebWindowNetCore.Bounds.Retrieve(settings.appId)
                : null;
            Size = new Size(bounds?.Width ?? settings.width, bounds?.Height ?? settings.height);
            WindowState = bounds?.IsMaximized == true ? FormWindowState.Maximized : FormWindowState.Normal;
        }
        isMaximized = WindowState == FormWindowState.Maximized;

        if (settings.saveBounds)
            FormClosing += OnClose;
        if (settings.canClose != null)
            FormClosing += OnCanClose;
        HandleCreated += OnHandle;
        Load += OnLoad;
        Text = settings.title;

        panel.Dock = DockStyle.Fill;
        panel.Controls.Add(WebView);
        Controls.Add(panel);

        ResumeLayout(false);

        Init();
        async void Init()
        {
            var env = await CoreWebView2Environment.CreateAsync(null, appDataPath, new CoreWebView2EnvironmentOptions(
                            customSchemeRegistrations: [
                                new CoreWebView2CustomSchemeRegistration("res")
                                {
                                    //TreatAsSecure = true,
                                    HasAuthorityComponent = true
                                }
                            ], additionalBrowserArguments: settings.withoutNativeTitlebar ? "--enable-features=msWebView2EnableDraggableRegions" : ""));
            await WebView.EnsureCoreWebView2Async(env);
            WebView.CoreWebView2.AddWebResourceRequestedFilter("res:*", CoreWebView2WebResourceContext.All);
            WebView.CoreWebView2.WebResourceRequested += ServeRes;
            WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            WebView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = settings.defaultContextMenuDisabled == false;
            WebView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            WebView.CoreWebView2.WebMessageReceived += WebMessageReceived;
            WebView.CoreWebView2.ContainsFullScreenElementChanged += OnFullscreen;
            WebView.CoreWebView2.WindowCloseRequested += (s, e) => Close();

            WebView.Source = new Uri(settings.GetUrl());

            await WebView.ExecuteScriptAsync(WebWindowNetCore.Windows.ScriptInjection.Get(settings.title));
            await Task.Delay(50);
            settings.RunJavascript($"WEBVIEWsetMaximized({(isMaximized ? "true" : "false")})");
            if (settings.withoutNativeTitlebar)
                Resize += (s, e) =>
                    {
                        if (WindowState == FormWindowState.Maximized != isMaximized)
                            isMaximized = WindowState == FormWindowState.Maximized;
                        settings.RunJavascript($"WEBVIEWsetMaximized({(isMaximized ? "true" : "false")})");
                    };
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

    void OnLoad(object? _, EventArgs e)
    {
        var bounds = WebWindowNetCore.Bounds.Retrieve(appId);
        if (bounds.X.HasValue && bounds.Y.HasValue)
            Location = new Point(bounds.X ?? 0, bounds.Y ?? 0);

        var screenBounds = Screen.FromRectangle(Bounds).WorkingArea;
        if (!screenBounds.Contains(Bounds))
        {
            // Adjust the form's location if it is out of bounds
            Location = new Point(Math.Max(screenBounds.X, Bounds.X), Math.Max(screenBounds.Y, Bounds.Y));
            // Ensure the form fits within the screen
            Size = new Size(Math.Min(screenBounds.Width, Bounds.Width), Math.Min(screenBounds.Height, Bounds.Height));
        }
        if (bounds.X.HasValue
                && bounds.Y.HasValue
                && Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(new Rectangle(bounds.X ?? 0, bounds.Y ?? 0, Size.Width, Size.Height))))
            Location = new Point(bounds.X ?? 0, bounds.Y ?? 0);
        Size = new Size(bounds.Width ?? width, bounds.Height ?? height);
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

    Unit SendTextResponse(int code, string status, string text, CoreWebView2WebResourceResponse response)
        => Unit
            .Value
            .SideEffect(_ =>
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
                response = WebView.CoreWebView2.Environment.CreateWebResourceResponse(stream, code,
                        status, $"Content-Type: text/plain");
            });

    Unit SendNotFound(CoreWebView2WebResourceResponse response)
        => SendTextResponse(404, "Not Found", "Resource not found", response);

    void ServeRes(object? _, CoreWebView2WebResourceRequestedEventArgs e)
    {
        try
        {
            var uri = "/" + Uri.UnescapeDataString(e.Request.Uri)[6..].SubstringAfter('/').SubstringUntil('?');
            uri = uri != "/" ? uri : "/index.html";
            var stream = Resources.Get(uri);

            if (stream != null)
            {
                try
                {
                    e.Response = WebView.CoreWebView2.Environment.CreateWebResourceResponse(stream, 200,
                        "OK", $"Content-Type: {uri.GetFileExtension()?.ToMimeType() ?? "text/html"}\nAccess-Control-Allow-Origin: *");
                }
                catch
                {
                    SendNotFound(e.Response);
                }
            }
        }
        catch
        {
            SendNotFound(e.Response);
        }
    }

    void WebMessageReceived(object? _, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var msg = e.TryGetWebMessageAsString();
        if (msg == "maximize")
            WindowState = FormWindowState.Maximized;
        else if (msg == "minimize")
            WindowState = FormWindowState.Minimized;
        else if (msg == "restore")
            WindowState = FormWindowState.Normal;
        else if (msg == "close")
            Close();
    }

    void OnFullscreen(object? s, object _)
    {
        if (WebView.CoreWebView2.ContainsFullScreenElement)
        {
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            //Taskbar.hide()
        }
        else
        {
            TopMost = false;
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.Sizable;
            //Taskbar.show ()
        }
    }

    void SetDarkMode(bool dark)
        => Invoke(() =>
            {
                var _ = Api.DwmSetWindowAttribute(Handle, DwmWindowAttribute.ImmersiveDarkMode, [dark ? 1 : 0], 4);
                BackColor = dark ? Color.Black : Color.White;
            });

    protected override void WndProc(ref Message m)
    {
        if (DesignMode || !withoutNativeTitlebar)
            base.WndProc(ref m);
        else
            switch (m.Msg)
            {
                case WM_NCCALCSIZE:
                    CalcSizeNoTitlebar(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }

        void CalcSizeNoTitlebar(ref Message m)
        {
            var isZoomedTop = Api.IsZoomed(Handle) ? 7 : 0;
            var isZoomedAll = Api.IsZoomed(Handle) ? 3 : 0;
            if (m.WParam != 0)
            {
                var nccsp = NcCalcSizeParams.Fromnint(m.LParam);
                nccsp.Rgrc0.Top += 1 + isZoomedTop;
                nccsp.Rgrc0.Bottom -= 5 + isZoomedAll;
                nccsp.Rgrc0.Left += 5 + isZoomedAll;
                nccsp.Rgrc0.Right -= 5 + isZoomedAll;
                Marshal.StructureToPtr(nccsp, m.LParam, true);
            }
            else
            {
                var clnRect = Marshal.PtrToStructure<Rect>(m.LParam);
                clnRect.Top += 1 + isZoomedTop;
                clnRect.Bottom -= 5 + isZoomedAll;
                clnRect.Left += 5 + isZoomedAll;
                clnRect.Right -= 5 + isZoomedAll;
                Marshal.StructureToPtr(clnRect, m.LParam, true);
            }
            m.Result = 0;
        }
    }

    bool isMaximized;
    const int WM_NCCALCSIZE = 0x83;
    readonly int width;
    readonly int height;
    readonly bool devTools;
    readonly Panel panel = new();
    readonly bool saveBounds;
    readonly string appId;
    readonly bool withoutNativeTitlebar;
    readonly Func<bool>? canClose;
}

#endif


    

