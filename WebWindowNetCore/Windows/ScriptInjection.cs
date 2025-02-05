namespace WebWindowNetCore.Windows;

static class ScriptInjection
{
    public static string Get(bool onFilesDrop) =>
$@"
{GetOnFilesDropScript(onFilesDrop)}

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
";
    static string GetOnFilesDropScript(bool activate) =>
        activate
        ? $@"
function dropFiles(id, move, droppedFiles) {{
    chrome.webview.postMessageWithAdditionalObjects({{
        msg: 1,
        text: id,
        move
    }}, droppedFiles)
}}"
        : $@"
function dropFiles() {{}}
";
}