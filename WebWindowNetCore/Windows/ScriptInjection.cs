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

function WEBVIEWsetMaximized(m) {{
    const maximize = document.getElementById('$MAXIMIZE$')
    if (maximize)
        maximize.style.display = m ? 'none' : ''

    const restore = document.getElementById('$RESTORE$')
    if (restore)
        restore.style.display = m ? '' : 'none'
}}

const title = document.getElementById('$TITLE$')
if (title)
    title.innerText = '{title}'
const close = document.getElementById('$CLOSE$')
if (close)
    close.onclick = () => window.close()
const maximize = document.getElementById('$MAXIMIZE$')
if (maximize) 
    maximize.onclick = () => window.chrome.webview.postMessage('maximize')
const minimize = document.getElementById('$MINIMIZE$')
if (minimize)
    minimize.onclick = () => window.chrome.webview.postMessage('minimize')
const restore = document.getElementById('$RESTORE$')
if (restore) {{
    restore.onclick = () => window.chrome.webview.postMessage('restore')
    restore.style.display = 'none'
}}
";
}