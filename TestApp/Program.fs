open System
open WebWindowNetCore
open WebWindow
open Commands

let iconPath = @"C:\Users\urieg\source\repos\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico"
//let iconPath = @"D:\Projekte\WebWindowNetCore\Native\webwindow.win.native\Tester\Brauser.ico"
let url = @"file://C:\Users\urieg\source\repos\WebWindowNetCore\WebRoot\index.html"
//let url = @"file://D:\Projekte\WebWindowNetCore\WebRoot\index.html"
//let url = @"file:///media/speicher/projekte/WebWindowNetCore/WebRoot/index.html"
// let url = "https://google.de"

let callback (text: string) = ()

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
        onEvent = callback
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
            Checkbox { Title = "&Versteckte Dateien"; Accelerator = Some "Strg+H"; OnChecked = onHidden; SetCheckedFunction = Some setSetHidden } 
            CmdItem { Title = "&Aktualisieren"; Accelerator = Some "Strg+R"; Action = onRefresh } 
            Separator 
            Checkbox { Title = "&Vorschau"; Accelerator = Some "F3"; OnChecked = onPreview; SetCheckedFunction = None } 
            Separator 
            Menu {
                Title = "&Themen"
                Items = [
                    MenuGroup {
                        OnSelected = onTheme
                        Items = [
                            Radio { Title = "&Blau"; Accelerator = None; Key = "blau"} 
                            Radio { Title = "&Hellblau"; Accelerator = None; Key = "hellblau"} 
                            Radio { Title = "&Dunkel"; Accelerator = None; Key = "dunkel" } 
                        ]
                    }
                ]
            }
            Separator 
            Menu {
                Title = "&Zoomlevel"
                Items = [
                    MenuGroup {
                        OnSelected = onZoom
                        Items = [ 
                            Radio { Title = "50%"; Accelerator = None; Key = 50 } 
                            Radio { Title = "75%"; Accelerator = None; Key = 75 } 
                            Radio { Title = "100%"; Accelerator = None; Key = 100 } 
                            Radio { Title = "150%"; Accelerator = None; Key = 150 } 
                            Radio { Title = "200%"; Accelerator = None; Key = 200 } 
                            Radio { Title = "250%"; Accelerator = None; Key = 250 } 
                            Radio { Title = "300%"; Accelerator = None; Key = 300 } 
                            Radio { Title = "350%"; Accelerator = None; Key = 350 } 
                            Radio { Title = "400%"; Accelerator = None; Key = 400 } 
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
setHidden true

execute () |> ignore
