namespace WebWindowNetCore

module RequestId = 
    open System.Threading
    let mutable private id = 0

    let get () =
        Interlocked.Increment &id