const btnDevTools = document.getElementById("buttonDevTools")
const dropZone = document.getElementById("dropZone")
const dragZone = document.getElementById("dragZone")

const onDragStart = async evt => { 
    dragZone.classList.add("blurry")
    evt.preventDefault()
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
    e.dataTransfer.dropEffect = e.shiftKey ? "move" : "copy"
})

dropZone.addEventListener("drop", e => {
    e.preventDefault()
    e.stopPropagation()
    WebView.dropFiles("dropZone", e.shiftKey, e.dataTransfer.files)
})

