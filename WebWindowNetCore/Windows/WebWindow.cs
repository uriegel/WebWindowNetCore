#if Windows

using CsTools.Extensions;
using Microsoft.Web.WebView2.WinForms;

namespace WebWindowNetCore.Windows;

public class WebWindow : WebWindowNetCore.WebWindow
{
    public override bool IsMaximized { get => Window.WindowState == FormWindowState.Maximized; }
    
    public override int Run()
    {
        Application.Run(Window);
        return 0;
    }

    public override void Close() => Window.Close();
    public override void Minimize() => Window.WindowState = FormWindowState.Minimized;
    public override void Maximize() => Window.WindowState = FormWindowState.Maximized;
    public override void Restore() => Window.WindowState = FormWindowState.Normal;
    public override void BeginInvoke(Action action) => Window.BeginInvoke(action);
    public override Task<T> InvokeAsync<T>(Func<T> func) => Window.InvokeAsync(func) ?? Task.FromResult<T>(default!);
    public override void SetFocus() => Window.BeginInvoke(async () =>
    {
        Window.Activate();
        Window.BringToFront();
        WebView.Focus();
        Window.Focus();
    });
    
    public override void ShowDevTools()
    {
        if (devTools)
            BeginInvoke(WebView.CoreWebView2.OpenDevToolsWindow);
    }

    public override async Task StartDragFiles(string[] fileList)
        => BeginInvoke(() => WebView.DoDragDrop(new DataObject(DataFormats.FileDrop, fileList), DragDropEffects.All));

    public override void RunJavascript(string script)
    {
        Run();
        async void Run()
        {
            try
            {
                await (WebView.ExecuteScriptAsync(script) ?? "".ToAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    
    public WebViewForm Window { get; }
    public WebView2 WebView { get; }

    internal WebWindow(WebViewForm window, WebView2 webView, WebWindowBuilder builder)
    {
        Window = window;
        WebView = webView;
        devTools = builder.devTools;
    }

    bool devTools;
}

#endif