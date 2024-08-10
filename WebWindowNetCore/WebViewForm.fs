namespace WebWindowNetCore
#if Windows
open System
open System.Drawing
open System.Windows.Forms
open Microsoft.Web.WebView2.Core
open Microsoft.Web.WebView2.WinForms
open FSharpTools

type WebViewForm(appDataPath: string, settings: WebViewBase) as this = 
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

        let bounds = Bounds.retrieve settings.AppIdValue
        if bounds.X.IsSome && bounds.Y.IsSome then
            this.Location <- Point(bounds.X |> Option.defaultValue 0, bounds.Y |> Option.defaultValue 0)
        this.Size <- Size(bounds.Width |> Option.defaultValue settings.WidthValue, bounds.Height |> Option.defaultValue settings.HeightValue)
        this.WindowState <- if bounds.IsMaximized then FormWindowState.Maximized else FormWindowState.Normal

        if settings.SaveBoundsValue then
            this.FormClosing.Add(this.onClosing)

        this.Load.Add(this.onLoad)

        base.Text <- settings.TitleValue
        base.Controls.Add webView

        (webView :> ComponentModel.ISupportInitialize).EndInit ()
        base.ResumeLayout false

        async {
            let! enf = CoreWebView2Environment.CreateAsync(null, appDataPath, null) |> Async.AwaitTask
            do! webView.EnsureCoreWebView2Async(enf) |> Async.AwaitTask
            webView.Source <- Uri (settings.GetUrl ())
        } 
        |> Async.StartWithCurrentContext 

    member this.onLoad (_: EventArgs) =
        let bounds = Bounds.retrieve settings.AppIdValue
        if bounds.X.IsSome && bounds.Y.IsSome 
                && Screen.AllScreens |> Seq.exists (fun s -> s.WorkingArea.IntersectsWith(Rectangle(
                                                                                                            bounds.X |> Option.defaultValue 0, 
                                                                                                            bounds.Y |> Option.defaultValue 0, 
                                                                                                            this.Size.Width, 
                                                                                                            this.Size.Height))) then
            this.Location <- Point(bounds.X |> Option.defaultValue 0 , bounds.Y |> Option.defaultValue 0)
            this.WindowState <- if bounds.IsMaximized then FormWindowState.Maximized else FormWindowState.Normal
        ()
    member this.onClosing (e: FormClosingEventArgs) =
            { Bounds.retrieve settings.AppIdValue with 
                X = if this.WindowState = FormWindowState.Maximized then Some this.RestoreBounds.Location.X else Some this.Location.X
                Y = if this.WindowState = FormWindowState.Maximized then Some this.RestoreBounds.Location.Y else Some this.Location.Y
                Width = if this.WindowState = FormWindowState.Maximized then Some this.RestoreBounds.Size.Width else Some this.Size.Width
                Height = if this.WindowState = FormWindowState.Maximized then Some this.RestoreBounds.Size.Height else Some this.Size.Height
                IsMaximized = this.WindowState = FormWindowState.Maximized }
            |> Bounds.save settings.AppIdValue
        
#endif    

    

