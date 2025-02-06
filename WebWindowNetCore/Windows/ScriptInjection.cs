namespace WebWindowNetCore.Windows;

static class ScriptInjection
{
    public static string Get(string title) =>
$@"
const showDevTools = () => window.chrome.webview.postMessage('showDevTools')    

function send_request(data) {{
    window.chrome.webview.postMessage(data)
}}

let startDragFilesBackRes = null
const startDragFiles = files => {{
    return new Promise(res => {{
        window.chrome.webview.postMessage('startDragFiles,' + JSON.stringify(files.map(n => n.replace('\\', '/'))))
        startDragFilesBackRes = res
    }})
}}
function startDragFilesBack() {{
    if (startDragFilesBackRes) {{
        startDragFilesBackRes()
        startDragFilesBackRes = null
    }}
}}

let droppedFilesBackRes = null
function droppedFilesBack(files) {{
    if (droppedFilesBackRes) {{
        droppedFilesBackRes(files) 
        droppedFilesBackRes = null
    }}
}}

function dropFiles(droppedFiles) {{
    return new Promise(res => {{
        droppedFilesBackRes = res
        chrome.webview.postMessageWithAdditionalObjects('droppedFiles', droppedFiles)
    }})
}}

const dragRegion = document.getElementById('$DRAG_REGION$')
if (dragRegion) {{
    dragRegion.style.setProperty('-webkit-app-region', 'drag')
    let activeElement = null
    dragRegion.onmousedown = e => {{
        activeElement = document.activeElement
    }}
    dragRegion.onmouseup = e => activeElement.focus()
}}

const title = document.getElementById('$TITLE$')
if (title)
    title.innerText = '{title}'
const close = document.getElementById('$CLOSE$')
if (close)
    close.onclick = () => window.close()
";
}