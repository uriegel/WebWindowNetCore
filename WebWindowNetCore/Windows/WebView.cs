#if Windows
using System.Diagnostics;
using System.Reflection;
using CsTools;
using CsTools.Extensions;
using ClrWinApi;

namespace WebWindowNetCore.Windows;

public class WebView : WebWindowNetCore.WebView
{
    public override int Run() 
    {
        Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
        Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        var loader = GetWebViewLoader(appId);
        var appDataPath = new FileInfo(loader).DirectoryName;
        Api.LoadLibrary(loader);

        var webForm = new WebViewForm(appDataPath!, this);
        Application.Run(webForm);
        return 0;
    }

    public override void ShowDevTools()
    {
        if (devTools)
            webViewForm?.BeginInvoke(() => webViewForm?.WebView.CoreWebView2.OpenDevToolsWindow());
    }

    public override async Task StartDragFiles(string[] fileList)
    {   
        webViewForm?.BeginInvoke(() => 
            webViewForm?.DoDragDrop(new DataObject(DataFormats.FileDrop, fileList), DragDropEffects.All)
        );
    }

    public override void RunJavascript(string script)
    {
        Run();
        async void Run()
        {
            try 
            {
                await (webViewForm?.WebView.ExecuteScriptAsync(script) ?? "".ToAsync());
            }
            catch (Exception ex)    
            {
                Console.WriteLine(ex.Message);
            }
        }        
    }

    string GetWebViewLoader(string appId)
    {
        var targetFileName = 
            Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .AppendPath(@$"{appId}\{Process.GetCurrentProcess().ProcessName}") 
                .EnsureDirectoryExists()
                .AppendPath("WebView2Loader.dll");
        using var targetFile = File.Create(targetFileName);
        Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("binaries/webviewloader")
            ?.CopyTo(targetFile);
        return targetFileName;
    }

    internal void Initialize(WebViewForm webViewForm) => this.webViewForm = webViewForm;

    WebViewForm? webViewForm;
}

#endif