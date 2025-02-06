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

dropZone.addEventListener("drop", async e => {
    e.preventDefault()
    e.stopPropagation()
    const res = await WebView.dropFiles(e.dataTransfer.files)
    console.log("files dropped", res)
})

