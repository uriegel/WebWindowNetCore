#if Windows
using CsTools.Extensions;
using Microsoft.Web.WebView2.WinForms;

namespace WebWindowNetCore.Windows;

/// <summary>
/// Run Script in WebView
/// </summary>
public static class Javascript
{
    /// <summary>
    /// /// Run Script in WebView
    /// </summary>
    /// <param name="script">Script, which should be run in WebView</param>
    public static void Run(string script)
    {
        Run();
        async void Run()
        {
            try 
            {
                await (webView?.ExecuteScriptAsync(script) ?? "".ToAsync());
            }
            catch (Exception ex)    
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
    internal static void Initialize(WebView2 webView) => Javascript.webView = webView;
    static WebView2? webView;
}
#endif