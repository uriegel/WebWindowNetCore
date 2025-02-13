#if Windows
namespace WebWindowNetCore.Windows;

public static class ScriptInjection
{
    public static string Get(string title) => 
$@"
//-Windows
function send_request(data) {{
    window.chrome.webview.postMessage(data)
}}

function WEBVIEWsetMaximized(m) {{
    const maximize = document.getElementById('$MAXIMIZE$')
    if (maximize)
        maximize.style.display = m ? 'none' : ''

    const restore = document.getElementById('$RESTORE$')
    if (restore)
        restore.style.display = m ? '' : 'none'
}}

function initializeCustomTitlebar() {{
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
}}

var WebView = (() => {{
    return {{
        request,
        backtothefuture,
        initializeCustomTitlebar,
    }}
}})()

try {{
    if (onWebViewLoaded) 
        onWebViewLoaded()
}} catch {{ }}

";
}
#endif