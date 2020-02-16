// Learn more about F# at http://fsharp.org

open System
open WebWindowNetCore

[<EntryPoint>]
let main argv =
    WebWindow.Run "https://google.de"
    printfn "Hello World from F#!"
    0 // return an integer exit code
