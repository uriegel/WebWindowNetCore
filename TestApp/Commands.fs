module Commands
open WebWindowNetCore

let onRename () = ()
let onExtendedRename () = ()
let onCopy () = ()
let onMove () = ()
let onDelete () = ()
let onCreatefolder () = ()
let onProperties () = ()
let onOpenWith () = ()
let onClose () = ()
let onFavorites () = ()
let onAdaptPath () = ()
let onSelectAll () = ()
let onDeselectAll () = ()
let onRefresh () = ()

let onDevTools () = WebWindow.sendToBrowser "Hallöschen Web Brauser😎😎👌"

let onHidden isChecked = ()
let mutable setHidden: bool -> unit = ignore
let setSetHidden (setHiddenFunction: bool -> unit) = setHidden <- setHiddenFunction

let onZoom (key: obj) = 
    let key = key :?> int
    ()
let mutable private setZoomFun: obj -> unit = ignore
let setSetZoom (setZoom: obj -> unit) = setZoomFun <- setZoom
let setZoom (key: int) = setZoomFun key

let onPreview isChecked = ()

let onTheme (key: obj) =
    let key = key :?> string
    ()
    // match key with
    // | "blau" -> setZoom 200
    // | "hellblau" -> setZoom 350
    // | "dunkel" -> setZoom 50
    // | _ -> ()
let mutable private setThemeFun: obj -> unit = ignore
let setSetTheme (setTheme: obj -> unit) = setThemeFun <- setTheme
let setTheme (key: string) = setThemeFun key


let onFullscreen () = setZoom 250