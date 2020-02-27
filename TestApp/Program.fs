open System
open WebWindowNetCore
open WebWindow
open Commands

//let iconPath = @"C:\Users\urieg\source\repos\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico"
let iconPath = @"D:\Projekte\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico"
//let url = @"file://C:\Users\urieg\source\repos\WebWindowNetCore\WebRoot\index.html"
let url = @"file://D:\Projekte\WebWindowNetCore\WebRoot\index.html"
//let url = @"file:///media/speicher/projekte/WebWindowNetCore/WebRoot/index.html"
// let url = "https://google.de"

let callback (text: string) = ()

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

let menu = [ 
    Menu {
        Title = "&Datei"
        Items = [ 
            CmdItem { Title = "&Umbenennen"; Accelerator = Some "F2"; Action = onRename } 
            CmdItem { Title = "&Erweitertes Umbenennen"; Accelerator = Some "Strg+F2"; Action = onExtendedRename } 
            Separator 
            CmdItem { Title = "&Kopieren"; Accelerator = Some "F5"; Action = onCopy } 
            CmdItem { Title = "&Verschieben"; Accelerator = Some "F6"; Action = onMove } 
            CmdItem { Title = "&Löschen"; Accelerator = Some "Entf"; Action = onDelete } 
            Separator 
            CmdItem { Title = "&Ordner anlegen"; Accelerator = Some "F7"; Action = onCreatefolder } 
            Separator 
            CmdItem { Title = "&Eigenschaften"; Accelerator = Some "Alt+Eingabe"; Action = onProperties } 
            CmdItem { Title = "&Öffnen mit"; Accelerator = Some "Strg+Eingabe"; Action = onOpenWith } 
            Separator 
            CmdItem { Title = "&Beenden"; Accelerator = Some "Alt+F4"; Action = onClose } 
        ]
    } 
    Menu {
        Title = "&Navigation"
        Items = [ 
            CmdItem { Title = "&Favoriten"; Accelerator = Some "F1"; Action = onFavorites } 
            CmdItem { Title = "&Gleichen Ordner öffnen"; Accelerator = Some "F9"; Action = onAdaptPath } 
        ]
    }
    Menu {
        Title = "&Selektion"
        Items = [ 
            CmdItem { Title = "&Alles"; Accelerator = Some "Num +"; Action = onSelectAll } 
            CmdItem { Title = "Alle &deselektieren"; Accelerator = Some "Num -"; Action = onDeselectAll } 
        ]
    }
    Menu {
        Title = "&Ansicht"
        Items = [ 
            Checkbox { Title = "&Versteckte Dateien"; Accelerator = Some "Strg+H" } 
            CmdItem { Title = "&Aktualisieren"; Accelerator = Some "Strg+R"; Action = onRefresh } 
            Separator 
            Checkbox { Title = "&Vorschau"; Accelerator = Some "F3" } 
            Separator 
            Menu {
                Title = "&Themen"
                Items = [
                    MenuGroup {
                        Items = [
                            Radio { Title = "&Blau"; Accelerator = None; } 
                            Radio { Title = "&Hellblau"; Accelerator = None; } 
                            Radio { Title = "&Dunkel"; Accelerator = None; } 
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
                            Radio { Title = "50%"; Accelerator = None } 
                            Radio { Title = "75%"; Accelerator = None } 
                            Radio { Title = "100%"; Accelerator = None } 
                            Radio { Title = "150%"; Accelerator = None } 
                            Radio { Title = "200%"; Accelerator = None } 
                            Radio { Title = "250%"; Accelerator = None } 
                            Radio { Title = "300%"; Accelerator = None } 
                            Radio { Title = "350%"; Accelerator = None } 
                            Radio { Title = "400%"; Accelerator = None } 
                        ]
                    }
                ]
            }
            CmdItem { Title = "Voll&bild"; Accelerator = Some "F11"; Action = onFullscreen } 
            Separator 
            CmdItem { Title = "&Entwicklungswerkzeuge"; Accelerator = Some "F12"; Action = onDevTools } 
        ]
    }
]

setMenu menu

execute () |> ignore
