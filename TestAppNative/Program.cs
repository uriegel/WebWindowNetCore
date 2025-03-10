﻿using WebWindowNetCore;

WebView
    .Create()
    .AppId("de.uriegel.test")
    .Title("Web Window Net Core Native 👍")
#if Linux    
    .FromResourceTemplate("template", TestAppNative.Linux.Window.Register)
#elif Windows
    .ResourceIcon("icon")
    .WithoutNativeTitlebar()
#endif        
    .InitialBounds(1200, 800)
    .SaveBounds()
    .FromResource()
    .DevTools()
    .CanClose(() => true)
    .Run();


