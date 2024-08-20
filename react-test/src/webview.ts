export declare type WebViewType = {
    showDevTools: () => void,
    startDragFiles: (files: string[]) => void,
    request: <T, TR>(method: string, data: T) => Promise<TR>
    registerEvents: <T>(id: string, evt: T) => void,
    dropFiles: (id: string, move: boolean, droppedFiles: string[]) => void,
    setDroppedFilesEventHandler: (success: boolean) => void
}

