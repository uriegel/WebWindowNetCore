namespace WebWindowNetCore.Linux;

static class ScriptInjection
{
    public static string Get() =>
$@"
function dropFiles() {{}}
function droppedFilesBack() {{}}
function initializeCustomTitlebar() {{}}

const showDevTools = () => fetch('req://showDevTools')    

function send_request(data) {{
    alert(data)
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


