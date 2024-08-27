console.log("script loaded")
const btn1 = document.getElementById("button")
const btn2 = document.getElementById("button2")
const btn3 = document.getElementById("button3")
const btnDevTools = document.getElementById("buttonDevTools")
const dragzone = document.getElementById("dragzone")

btnDevTools.onclick = () => WebView.showDevTools()

let currentDirectory = ""

const initialize = async () => {
    WebView.registerEvents("fast", console.log)
    WebView.registerEvents("slow", console.log)
    WebView.setDroppedFilesEventHandler(success => console.log("Files dropped", success))
    currentDirectory = (await WebView.request("getCurrentDir", {})).directory
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

dragzone.onmousedown = () => WebView.startDragFiles([
        "TestApp.dll",
        "FSharpTools.dll"
    ]
    .map(n => `${currentDirectory}${n}`)
)
