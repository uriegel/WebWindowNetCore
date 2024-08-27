namespace WebWindowNetCore

module internal Script = 
    let get noNativeTitlbar title port windows doFilesDrop =

        let devTools = 
            if windows then
                "const showDevTools = () => callback.ShowDevtools()
                    const startDragFiles = files => callback.StartDragFiles(JSON.stringify({ files }))"
            else
                "const showDevTools = () => fetch('req://showDevTools')
                    const startDragFiles = files => fetch('req://startDragFiles', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ files })
                    })"

        let onEventsCreated = 
            if windows then
                "const onEventsCreated = id => callback.OnEvents(id)"
            else
                "const onEventsCreated = id => fetch(`req://onEvents/${id}`)"

        let noTitlebarScript = 
            if noNativeTitlbar then 
                sprintf """
                    function WEBVIEWsetMaximized(m) { 
                        const maximize = document.getElementById('$MAXIMIZE$')
                        if (maximize)
                            maximize.hidden = m

                        const restore = document.getElementById('$RESTORE$')
                        if (restore)
                            restore.hidden = !m
                    }

                    const WEBVIEWNoNativeTitlebarInitialize = () => {
                        const favicon = document.getElementById('$FAVICON$')
                        if (favicon)
                            favicon.src = 'res://favicon'
                        const title = document.getElementById('$TITLE$')
                        if (title)
                            title.innerText = "%s"
                        const close = document.getElementById('$CLOSE$')
                        if (close)
                            close.onclick = () => window.close()
                        const maximize = document.getElementById('$MAXIMIZE$')
                        if (maximize) 
                            maximize.onclick = () => callback.MaximizeWindow()
                        const minimize = document.getElementById('$MINIMIZE$')
                        if (minimize)
                            minimize.onclick = () => callback.MinimizeWindow()
                        const restore = document.getElementById('$RESTORE$')
                        if (restore) {
                            restore.onclick = () => callback.RestoreWindow()
                            restore.hidden = true
                        }
                        const hamburger = document.getElementById('$HAMBURGER$')
                        if (hamburger) 
                            hamburger.onclick = () => callback.OnHamburger(hamburger.offsetLeft / document.body.offsetWidth, (hamburger.offsetTop + hamburger.offsetHeight) / document.body.offsetHeight)
                            
                    }
                    WEBVIEWNoNativeTitlebarInitialize()
                """ title
            else
                ""

        let onFilesDropScript =
            if doFilesDrop then
                @"function dropFiles(id, move, droppedFiles) {
                    chrome.webview.postMessageWithAdditionalObjects({
                        msg: 1,
                        text: id,
                        move
                    }, droppedFiles)
                }"
            else
                "function dropFiles() {}"
        
        sprintf """
            %s

            var webViewEventSinks = new Map()

            var WebView = (() => {
                %s
                %s
                %s
                const request = async (method, data) => {
                    const res = await fetch(`http://localhost:%d/requests/${method}`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify(data)
                    })
                    return await res.json()
                }

                const registerEvents = (id, evt) => {
                    webViewEventSinks.set(id, evt)
                    onEventsCreated(id)
                }

                let evtHandler = () => { }
                const setDroppedFilesEventHandler = eh => evtHandler = eh

                const setDroppedEvent = success => evtHandler(success)

                initializeNoTitlebar = () => WEBVIEWNoNativeTitlebarInitialize()

                return {
                    initializeNoTitlebar,
                    showDevTools,
                    startDragFiles,
                    request,
                    registerEvents,
                    dropFiles,
                    setDroppedFilesEventHandler,
                    setDroppedEvent
                }
            })()

            try {
                if (onWebViewLoaded) 
                    onWebViewLoaded()
            } catch { }
        """ noTitlebarScript devTools onFilesDropScript onEventsCreated port
