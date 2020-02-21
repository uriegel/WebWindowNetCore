open System
open WebWindowNetCore

[<EntryPoint>]
let main argv =
    let (configuration: WebWindow.Configuration) = {
        title = "Web brauser😎😎👌"
        url = "www.google.de" 
    }
    
    WebWindow.initialize configuration
    WebWindow.execute ()
