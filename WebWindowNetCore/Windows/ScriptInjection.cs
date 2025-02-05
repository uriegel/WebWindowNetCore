namespace WebWindowNetCore.Windows;

static class ScriptInjection
{
    public static string Get() =>
$@"
function send_request(data) {{
    window.chrome.webview.postMessage(data)
}}

let startDragFilesBackRes = null
const startDragFiles = files => {{
    return new Promise(res => {{
        fetch('req://startDragFiles', {{
            method: 'POST',
            headers: {{ 'Content-Type': 'application/json' }},
            body: JSON.stringify({{files}})
        }})
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


