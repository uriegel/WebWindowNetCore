console.log("script loaded")
const btn1 = document.getElementById("button")
const btn2 = document.getElementById("button2")
const btn3 = document.getElementById("button3")
const btnDevTools = document.getElementById("buttonDevTools")
const dragzone = document.getElementById("dragzone")

btn1.focus()

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

window.onload = function() {
    btn1.focus()
}

btn1.onclick = async () => {
    var res = await WebView.request("cmd1", {
        text: "Text",
        id: 123
    })

    const text = document.getElementById("text")
    text.innerText = res.name

    console.log("cmd1", res)
}

btn2.onclick = async () => {
    var res = await WebView.request("cmd2", {})
    console.log("cmd2", res)
}

btnDevTools.onclick = () => webViewRequest("showdevtools")

dragzone.onmousedown = async () => {
    await webViewRequest("startdragfiles", [
            "README.md",
            "LICENSE"
        ]
        .map(n => `${currentDirectory}${n}`))
    console.log("Drag finished")
}

const webViewRequest = async (method, payload) => {

    const msg = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }

    const response = await fetch(`http://localhost:8080/json/${method}`, msg) 
    const res = await response.json() 
    if (res.err)
        throw new RequestError(res.err.status, res.err.statusText)
    return res.ok 
}



