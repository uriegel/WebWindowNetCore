namespace WebWindowNetCore
#if Linux
open GtkDotNet

type WebView() = 
    inherit WebViewBase()
    
    override this.Run() =
        Application
            .NewAdwaita(this.AppIdValue)
            .OnActivate(fun app ->
                Application
                    .NewWindow(app) 
                    .Title(this.TitleValue)
                    .DefaultSize(this.WidthValue, this.HeightValue)
                    .Child(WebKit.New()
                        //.Ref())
                        .LoadUri("https://google.de"))
                    .Show()
                    |> ignore)
            .Run(0, 0)
#endif