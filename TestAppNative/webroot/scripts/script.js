const btnDevTools = document.getElementById("buttonDevTools")
const dropZone = document.getElementById("dropZone")
const dragZone = document.getElementById("dragZone")

const onDragStart = async evt => { 
    dragZone.classList.add("blurry")
    evt.preventDefault()
//    await webViewDragStart("C:\\Users\\urieg\\test2", ["affe.config", "affen.config"])
    await webViewDragStart("D:\\CaesarTeams", ["CaesarTeams.exe", "clrcompression.dll"])
    dragZone.classList.remove("blurry")
}

btnDevTools.onclick = () => WebView.showDevTools()

dragZone.ondragstart = onDragStart    

document.body.addEventListener("dragover", e => {
    e.preventDefault()
    e.stopPropagation()
    e.dataTransfer.dropEffect = "none"
})

dropZone.addEventListener("dragover", e => {
    e.preventDefault()
    e.stopPropagation()
    e.dataTransfer.dropEffect = evt.shiftKey ? "move" : "copy"
})

dropZone.addEventListener("drop", e => {
    e.preventDefault()
    e.stopPropagation()
    webViewDropFiles("dropZone", true, e.dataTransfer.files)
})

