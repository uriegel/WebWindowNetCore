using WebWindowNetCore;
#if Linux
using GtkDotNet;
#endif
new WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("Web Window Net Core 👍")
    .ResourceIcon("icon")
    .ResourceScheme()
    .SaveBounds()
    .DefaultContextMenuDisabled()
#if DEBUG    
    .DevTools()
#endif
    .Url("res://webroot/index.html")
#if Linux
    .TitleBar((a, w, wv) => HeaderBar.New()
                            .PackEnd(
                                ToggleButton.New()
                                .IconName("open-menu-symbolic")
                                .OnClicked(() => wv.Ref.GrabFocus())
                            ))
#endif
    .Run();
