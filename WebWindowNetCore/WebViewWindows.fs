namespace WebWindowNetCore
#if Windows
open System.Diagnostics
open System.Windows.Forms
open System.Threading
open System
open System.Reflection

open FSharpTools
open System.IO
open ClrWinApi

type WebView() = 
    inherit WebViewBase()

    let getWebViewLoader () =
        let targetFileName = 
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            |> Directory.attachSubPath (sprintf "de.uriegel.WebWindowNetCore\%s" <|Process.GetCurrentProcess().ProcessName)
            |> Directory.ensureExists
            |> Directory.attachSubPath "WebView2Loader.dll"
        try 
            use targetFile = File.Create targetFileName
            Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("binaries/webviewloader")
                .CopyTo targetFile
                |> ignore
        with 
        | _ -> ()
        targetFileName
    
    override this.Run() =

        Thread.CurrentThread.SetApartmentState ApartmentState.Unknown
        Thread.CurrentThread.SetApartmentState ApartmentState.STA

        Application.EnableVisualStyles ()
        Application.SetCompatibleTextRenderingDefault false
        Application.SetHighDpiMode HighDpiMode.PerMonitorV2 |> ignore

        let loader = getWebViewLoader ()
        let appDataPath = FileInfo(loader).DirectoryName
        Api.LoadLibrary loader |> ignore

        let webForm = new WebViewForm(appDataPath, this)
        Application.Run(webForm) 
        0

#endif