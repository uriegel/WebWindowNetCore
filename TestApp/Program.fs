open System
open WebWindowNetCore

[<EntryPoint>]
let main argv =
    let (configuration: WebWindow.Configuration) = {
        title = "Web brauser😎😎👌"
        url = "https://www.google.de" 
    }
    
    WebWindow.initialize configuration
    WebWindow.execute ()
