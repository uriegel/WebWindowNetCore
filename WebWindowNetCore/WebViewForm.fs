namespace WebWindowNetCore
#if Windows
open System
open System.Drawing
open System.Windows.Forms
open Microsoft.Web.WebView2.Core
open Microsoft.Web.WebView2.WinForms
open FSharpTools

type WebViewForm(appDataPath: string, settings: WebViewBase) = 
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

        base.Text <- settings.TitleValue
        base.Size <- Size (settings.WidthValue, settings.HeightValue)
        base.Controls.Add webView

        (webView :> ComponentModel.ISupportInitialize).EndInit ()
        base.ResumeLayout false

        async {
            let! enf = CoreWebView2Environment.CreateAsync(null, appDataPath, null) |> Async.AwaitTask
            do! webView.EnsureCoreWebView2Async(enf) |> Async.AwaitTask
            webView.Source <- Uri (settings.GetUrl ())
        } 
        |> Async.StartWithCurrentContext 


#endif    

    

