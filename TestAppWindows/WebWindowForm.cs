using CsTools.Extensions;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using WebWindowNetCore.Data;

namespace WebWindowNetCore;

public class WebWindowForm : Form
{
    public void Init(int width, int height, bool maximize)  
    {
        ClientSize = new System.Drawing.Size(width, height);
        if (maximize)
            WindowState = FormWindowState.Maximized;
        
    }

    public void ShowDevtools()
        => webView.CoreWebView2.OpenDevToolsWindow();
    
    public WebWindowForm(WebViewSettings? settings, string appDataPath) 
    {
        webView = new WebView2();
        ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
        SuspendLayout();
        // 
        // webView
        // 
        this.webView.AllowExternalDrop = true;
        this.webView.CreationProperties = null;
        this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
        this.webView.Location = new System.Drawing.Point(0, 0);
        this.webView.Margin = new System.Windows.Forms.Padding(0);
        this.webView.Name = "webView";
        this.webView.Dock = DockStyle.Fill;
        this.webView.TabIndex = 0;
        this.webView.ZoomFactor = 1D;
        // 
        // Form1
        // 
        //this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        
        if (settings?.ResourceIcon != null)
            this.Icon = new System.Drawing.Icon(Resources.Get(settings.ResourceIcon)!);

        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.WindowState = FormWindowState.Minimized;
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        
        this.Controls.Add(this.webView);
        this.Name = "WebWindow";

        this.Text = settings?.Title;
        ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
        this.ResumeLayout(false);

        this.Resize += async (s, e) =>
        {
            if (initialized && settings?.SaveBounds == true) {
                if (this.WindowState != FormWindowState.Maximized)
                    await webView.ExecuteScriptAsync(
                        $$"""
                            localStorage.setItem('window-bounds', JSON.stringify({width: {{Width}}, height: {{Height}}}))
                            localStorage.setItem('isMaximized', false)
                        """);
                else
                    await webView.ExecuteScriptAsync($"localStorage.setItem('isMaximized', true)");
            }
        };

        StartWebviewInit();

        async void StartWebviewInit()
        {
            var enf = await  CoreWebView2Environment.CreateAsync(null, appDataPath);
            await webView.EnsureCoreWebView2Async(enf);
            webView.CoreWebView2.AddHostObjectToScript("Callback", new Callback(this));
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            
            webView.CoreWebView2.ContainsFullScreenElementChanged += (objs, args) =>
            {
                if (webView.CoreWebView2.ContainsFullScreenElement)
                {
                    this.TopMost = true;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    Taskbar.Hide();
                }
                else
                {
                    this.TopMost = false;
                    this.WindowState = FormWindowState.Normal;
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    Taskbar.Show();
                }
            };
                
            if (settings?.Url != null)
                webView.Source = new System.Uri(settings?.Url ?? "");
            if (settings?.HttpSettings?.WebrootUrl != null)
            {
#if DEBUG
                var uri = string.IsNullOrEmpty(settings?.DebugUrl)
                    ? $"http://localhost:{settings?.HttpSettings?.Port ?? 80}{settings?.HttpSettings?.WebrootUrl}/{settings?.HttpSettings?.DefaultHtml}"
                    : settings?.DebugUrl!;
#else
                var uri = $"http://localhost:{settings?.HttpSettings?.Port ?? 80}{settings?.HttpSettings?.WebrootUrl}/{settings?.HttpSettings?.DefaultHtml}";
#endif                
                webView.Source = new System.Uri(uri);
            }

            WindowState = FormWindowState.Normal;
            initialized = true;
            await webView.ExecuteScriptAsync(
                $$"""
                    const callback = chrome.webview.hostObjects.Callback
                    callback.Ready()
                    const bounds = JSON.parse(localStorage.getItem('window-bounds') || '{}')
                    const isMaximized = localStorage.getItem('isMaximized')
                    callback.Init(bounds.width || {{settings?.Width ?? 800}}, bounds.height || {{settings?.Height ?? 600}}, isMaximized == 'true')
                """);
            if (settings?.DevTools == true)
                await webView.ExecuteScriptAsync(
                    """ 
                        function webViewShowDevTools() {
                            callback.ShowDevtools()
                        }
                    """);
            if ((settings?.HttpSettings?.RequestDelegates?.Length ?? 0) > 0)
                await webView.ExecuteScriptAsync(
                    """ 
                        async function webViewRequest(method, input) {

                            const msg = {
                                method: 'POST',
                                headers: { 'Content-Type': 'application/json' },
                                body: JSON.stringify(input)
                            }

                            const response = await fetch(`/request/${method}`, msg) 
                            return await response.json() 
                        }
                    """);                
        }
    }


    WebView2 webView;
    bool initialized;
}
