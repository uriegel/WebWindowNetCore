namespace WebWindowNetCore.Windows;

static class ScriptInjection
{
    public static string Get() =>
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
";
}


