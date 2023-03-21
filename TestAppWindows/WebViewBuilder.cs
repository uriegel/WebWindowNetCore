using System.Reflection;
using CsTools.Extensions;
using WebWindowNetCore.Data;

using static ClrWinApi.Api;

namespace WebWindowNetCore;

public class WebViewBuilder : WebWindowNetCore.Base.WebViewBuilder
{
    public override WebView Build() => new WebView(this);

    public string AppDataPath { get; }

    internal new WebViewSettings Data { get => base.Data; }

    internal WebViewBuilder()
    {
        Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
        Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(defaultValue: false);
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        var loader = GetWebViewLoader();
        AppDataPath = new FileInfo(loader).DirectoryName!;
        LoadLibrary(loader);

        string GetWebViewLoader()
        {
            using var targetFile = 
                Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .AppendPath("uriegel.WebWindowNetCore")
                .EnsureDirectoryExists()
                .AppendPath("WebView2Loader.dll")
                .CreateFile();
            Assembly
                .GetExecutingAssembly()
                ?.GetManifestResourceStream("binaries/webviewloader")
                ?.CopyTo(targetFile);
            return (targetFile as FileStream)!.Name;
        }
    }
}