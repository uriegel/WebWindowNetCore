console.log("script loaded")

const btn1 = document.getElementById("button")
const btn2 = document.getElementById("button2")
const btnDevTools = document.getElementById("buttonDevTools")

btnDevTools.onclick = WebView.showDevTools

WebView.registerEvents("fast", console.log)
WebView.registerEvents("slow", console.log)

btn1.onclick = async () => {
    var res = await WebView.request("cmd1", {
        text: "Text",
        id: 123
    })
    console.log("cmd1", res)
}

btn2.onclick = async () => {
    var res = await WebView.request("cmd2", {
        eMail: "uriegel@github.com",
        nr: 1123
    })
    console.log("cmd2", res)
}


