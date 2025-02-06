console.log("script loaded")
const btn1 = document.getElementById("button")
const btn2 = document.getElementById("button2")
const btn3 = document.getElementById("button3")
const btnDevTools = document.getElementById("buttonDevTools")
const dragzone = document.getElementById("dragzone")

btnDevTools.onclick = () => WebView.showDevTools()

let currentDirectory = ""

const initialize = async () => {
    res = await WebView.request("cmd3", {})
    currentDirectory = res.path
    console.log("baseDirectory", currentDirectory)
}
try {
    if (WebView)
        initialize()
} catch {  }

function onWebViewLoaded() {
    initialize()
}

btn1.onclick = async () => {
    var res = await WebView.request("cmd1", {
        text: "Text",
        id: 123
    })
    console.log("cmd1", res)
}

btn2.onclick = async () => {
    var res = await WebView.request("cmd2", {})
    console.log("cmd2", res)
}

btn3.onclick = () => alert("A message from javascript")

dragzone.onmousedown = async () => {
    await WebView.startDragFiles([
            "README.md",
            "LICENSE"
        ]
        .map(n => `${currentDirectory}${n}`))
    console.log("Drag finished")
}

