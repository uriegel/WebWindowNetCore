// Learn more about F# at http://fsharp.org

open System
open WebWindowNetCore

[<EntryPoint>]
let main argv =
    Say.hello "Sag Hallo"
    printfn "Hello World from F#!"
    0 // return an integer exit code
