#if Windows
using System.Diagnostics;
using System.Reflection;
using CsTools;
using CsTools.Extensions;

namespace WebWindowNetCore.Windows;

public class WebView : WebWindowNetCore.WebView
{
    public WebView()
    {
        var targetFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                        .AppendPath(@$"{appId}\{Process.GetCurrentProcess().ProcessName}") 
                        .EnsureDirectoryExists()
                        .AppendPath("WebView2Loader.dll");
        using var targetFile = File.Create(targetFileName);
        Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("binaries/webviewloader")
            ?.CopyTo(targetFile);
    }
    public override int Run() 
        => 1;
}

#endif