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
let onFullscreen () = ()
let onDevTools () = WebWindow.sendToBrowser "Hallöschen Web Brauser😎😎👌"

let onHidden isChecked = ()
let mutable setHidden: bool -> unit = ignore
let setSetHidden (setHiddenFunction: bool -> unit) = setHidden <- setHiddenFunction

let onTheme (key: obj) =
    let key = key :?> string
    ()

let onZoom (key: obj) = 
    let key = key :?> int
    ()

let onPreview isChecked = ()
