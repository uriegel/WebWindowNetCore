#if Windows

using System.Diagnostics;
using System.Reflection;
using ClrWinApi;
using CsTools;
using CsTools.Extensions;

namespace WebWindowNetCore.Windows;

public class WebWindowBuilder : WebWindowNetCore.WebWindowBuilder
{
    public override WebWindowNetCore.WebWindow Build()
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
        webForm.WebWindow = new WebWindow(webForm, webForm.WebView, this);
        onCreate?.Invoke(webForm.WebWindow);

        return webForm.WebWindow;
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
}

#endif

