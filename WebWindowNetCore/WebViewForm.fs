namespace WebWindowNetCore
#if Windows
open System
open System.Drawing
open System.Windows.Forms
open Microsoft.Web.WebView2.Core
open Microsoft.Web.WebView2.WinForms
open System.Threading

type WebViewForm(appDataPath: string) = 
    inherit Form()

    let webView = new WebView2()
    
    do 
        (webView :> ComponentModel.ISupportInitialize).BeginInit()
        webView.DefaultBackgroundColor <- Color.White
        webView.Location <- Point (0, 0)
        webView.Margin <- Padding 0
        webView.Dock <- DockStyle.Fill
        webView.TabIndex <- 0
        webView.ZoomFactor <- 1

        base.AutoScaleDimensions <- SizeF(8F, 20F)
        base.AutoScaleMode <- AutoScaleMode.Font

        base.Text <- "Hallo"
        base.Size <- Size (800, 600)
        base.Controls.Add webView

        (webView :> ComponentModel.ISupportInitialize).EndInit ()
        base.ResumeLayout false

        let startInContext (sync: SynchronizationContext) work = 
            sync.Send((fun _ -> Async.StartImmediate(work)), null)

        async {
            let! enf = CoreWebView2Environment.CreateAsync(null, appDataPath, null) |> Async.AwaitTask
            do! webView.EnsureCoreWebView2Async(enf) |> Async.AwaitTask
            webView.Source <- Uri ("https://google.de")
        } 
        |> startInContext SynchronizationContext.Current


#endif    

    

