namespace WebWindowNetCore
#if Windows
open System.Windows.Forms

type WebView() = 
    inherit WebViewBase()
    
    override this.Run() =
        let webForm = new Form()
        Application.Run(webForm) 
        0

#endif