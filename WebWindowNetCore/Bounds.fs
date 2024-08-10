namespace WebWindowNetCore
open System
open System.IO
open FSharpTools
open FSharpTools.Functional

type Bounds = {
    X: int option
    Y: int option
    Width: int option
    Height: int option
    IsMaximized: bool
}

module internal Bounds = 
    let private getPath id = 
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        |> Directory.attachSubPath id
        |> sideEffectIf (fun p-> Directory.Exists(p) = false) Directory.CreateDirectory
        |> Directory.attachSubPath "bounds.json"

    let retrieve id =
        getPath id
        |> File.readAllText 
        |> Option.defaultValue ""
        |> sideEffect (printfn  "%s")
        |> TextJson.deserialize<Bounds>
    
    let save id bounds =
        getPath id
        |> File.writeAllText (TextJson.serialize<Bounds> bounds) 
