namespace WebWindowNetCore;

public static class ScriptInjection
{
    public static string Get(bool windows, string title) => 
$@"
{(windows ? Windows.ScriptInjection.Get(title) : Linux.ScriptInjection.Get())}

var webviewrequestsid = 0
var webviewrequests = new Map()

const backtothefuture = (data) => {{
    if (data.startsWith('result,')) {{
        const msg = data.substring(7)
        const idx = msg.indexOf(',')
        const id = msg.substring(0, idx)
        const back = msg.substring(idx + 1).replace('u0027', ""'"")
        const json = JSON.parse(back)
        const res = webviewrequests.get(id)    
        webviewrequests.delete(id)
        res(json)
    }} else
        console.log('Message received', data)
}}
    
const request = (method, data) => new Promise(res => {{
    webviewrequests.set((++webviewrequestsid).toString(), res)
    const msg = `request,${{method}},${{webviewrequestsid}},${{JSON.stringify(data)}}`
    send_request(msg)
}})

var WebView = (() => {{
    return {{
        showDevTools,
        startDragFiles,
        request,
        backtothefuture,
        startDragFilesBack,
        dropFiles,
        droppedFilesBack,
        initializeCustomTitlebar,
    }}
}})()

try {{
    if (onWebViewLoaded) 
        onWebViewLoaded()
}} catch {{ }}

";
}