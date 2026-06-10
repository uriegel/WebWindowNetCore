const btnDevTools = document.getElementById("buttonDevTools")
btnDevTools.onclick = () => WebView.showDevTools()

const dropZone = document.getElementById("dropZone")

const initialize = async () => {
    WebView.initializeCustomTitlebar()
}
try {
    if (WebView)
        initialize()
} catch {  }
function onWebViewLoaded() {
    initialize()
}

btnDevTools.focus()

document.body.addEventListener("dragover", e => {
    e.preventDefault()
    e.stopPropagation()
    e.dataTransfer.dropEffect = "none"
})

dropZone.addEventListener("dragover", e => {
    e.preventDefault()
    e.stopPropagation()
    e.dataTransfer.dropEffect = e.shiftKey ? "move" : "copy"
})

dropZone.addEventListener("drop", async e => {
    e.preventDefault()
    e.stopPropagation()
    const res = await WebView.dropFiles(e.dataTransfer.files)
    console.log("files dropped", res)
})

