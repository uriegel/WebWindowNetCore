namespace WebWindowNetCore
#if Windows
open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Threading
open System.Windows.Forms

open FSharpTools
open FSharpTools.Functional
open ClrWinApi

type internal WebView() = 
    inherit WebViewBase()

    let getWebViewLoader () =
        let targetFileName = 
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            |> Directory.attachSubPath (sprintf "de.uriegel.WebWindowNetCore\%s" <|Process.GetCurrentProcess().ProcessName)
            |> sideEffectIf Directory.Exists Directory.CreateDirectory
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