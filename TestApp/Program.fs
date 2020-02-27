open System
open WebWindowNetCore
open WebWindow

let iconPath = @"C:\Users\urieg\source\repos\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico"
//let url = @"file://C:\Users\urieg\source\repos\WebWindowNetCore\WebRoot\index.html"
let url = @"file://D:\Projekte\WebWindowNetCore\WebRoot\index.html"
//let url = @"file:///media/speicher/projekte/WebWindowNetCore/WebRoot/index.html"
// let url = "https://google.de"

let callback (text: string) =
        printfn "Das kam vom lieben Webview: %s" text
        let t = text
        ()
let callbackDelegate = Callback callback

let configuration = { 
    defaultConfiguration () with
        title = "Web brauser😎😎👌"
        url = url
        iconPath = iconPath
        debuggingEnabled = true
        organization = "URiegel"
        application = "TestBrauser"
        saveWindowSettings = true
        fullScreenEnabled = true
        callback = callbackDelegate
}

initialize configuration

async {
    let rec readLine () = 
        let line = Console.ReadLine ()
        sendToBrowser line
        readLine ()
    readLine ()
} |> Async.Start

let menu = [ 
    Menu {
        Title = "&Datei"
        Items = [ 
            CmdItem { Title = "&Umbenennen"; Accelerator = Some "F2"; Cmd = 1 } 
            CmdItem { Title = "&Erweitertes Umbenennen"; Accelerator = Some "Strg+F2"; Cmd = 2 } 
            Separator 
            CmdItem { Title = "&Kopieren"; Accelerator = Some "F5"; Cmd = 3 } 
            CmdItem { Title = "&Verschieben"; Accelerator = Some "F6"; Cmd = 4 } 
            CmdItem { Title = "&Löschen"; Accelerator = Some "Entf"; Cmd = 5 } 
            Separator 
            CmdItem { Title = "&Ordner anlegen"; Accelerator = Some "F7"; Cmd = 6 } 
            Separator 
            CmdItem { Title = "&Eigenschaften"; Accelerator = Some "Alt+Eingabe"; Cmd = 7 } 
            CmdItem { Title = "&Öffnen mit"; Accelerator = Some "Strg+Eingabe"; Cmd = 8 } 
            Separator 
            CmdItem { Title = "&Beenden"; Accelerator = Some "Alt+F4"; Cmd = 9 } 
        ]
    } 
    Menu {
        Title = "&Navigation"
        Items = [ 
            CmdItem { Title = "&Favoriten"; Accelerator = Some "F1"; Cmd = 10 } 
            CmdItem { Title = "&Gleichen Ordner öffnen"; Accelerator = Some "F9"; Cmd = 11 } 
        ]
    }
    Menu {
        Title = "&Selektion"
        Items = [ 
            CmdItem { Title = "&Alles"; Accelerator = Some "Num +"; Cmd = 12 } 
            CmdItem { Title = "Alle &deselektieren"; Accelerator = Some "Num -"; Cmd = 13 } 
        ]
    }
    Menu {
        Title = "&Ansicht"
        Items = [ 
            Checkbox { Title = "&Versteckte Dateien"; Accelerator = Some "Strg+H"; Cmd = 14 } 
            CmdItem { Title = "&Aktualisieren"; Accelerator = Some "Strg+R"; Cmd = 15 } 
            Separator 
            Checkbox { Title = "&Vorschau"; Accelerator = Some "F3"; Cmd = 16 } 
            Separator 
            Menu {
                Title = "&Themen"
                Items = [
                    MenuGroup {
                        Items = [
                            CmdItem { Title = "&Blau"; Accelerator = None; Cmd = 17 } 
                            CmdItem { Title = "&Hellblau"; Accelerator = None; Cmd = 18 } 
                            CmdItem { Title = "&Dunkel"; Accelerator = None; Cmd = 19 } 
                        ]
                    }
                ]
            }
            Separator 
            Menu {
                Title = "&Zoomlevel"
                Items = [
                    MenuGroup {
                        Items = [ 
                            CmdItem { Title = "50%"; Accelerator = None; Cmd = 20 } 
                            CmdItem { Title = "75%"; Accelerator = None; Cmd = 21 } 
                            CmdItem { Title = "100%"; Accelerator = None; Cmd = 22 } 
                            CmdItem { Title = "150%"; Accelerator = None; Cmd = 23 } 
                            CmdItem { Title = "200%"; Accelerator = None; Cmd = 24 } 
                            CmdItem { Title = "250%"; Accelerator = None; Cmd = 25 } 
                            CmdItem { Title = "300%"; Accelerator = None; Cmd = 26 } 
                            CmdItem { Title = "350%"; Accelerator = None; Cmd = 27 } 
                            CmdItem { Title = "400%"; Accelerator = None; Cmd = 28 } 
                        ]
                    }
                ]
            }
            CmdItem { Title = "Voll&bild"; Accelerator = Some "F11"; Cmd = 29 } 
            Separator 
            CmdItem { Title = "&Entwicklungewerkzeuge"; Accelerator = Some "F12"; Cmd = 30 } 
        ]
    }
]

setMenu menu

execute () |> ignore
