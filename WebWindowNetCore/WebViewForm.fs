namespace WebWindowNetCore
#if Windows
open System.Windows.Forms
open Microsoft.Web.WebView2.WinForms

type WebViewForm() = 
    inherit Form()

    let webView = new WebView2()
    
    do 
        (webView :> System.ComponentModel.ISupportInitialize).BeginInit()
        base.Text <- "Hallo"

#endif    

    

