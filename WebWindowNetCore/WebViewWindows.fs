namespace WebWindowNetCore
#if Windows
open System.Windows.Forms

type WebView() = 
    inherit WebViewBase()
    
    override this.Run() =
        let webForm = new WebViewForm()
        Application.Run(webForm) 
        0

#endif