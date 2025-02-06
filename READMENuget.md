# WebWindowNetCore
A .NET 8 Webview Application for Windows and Linux similar to Electron. It uses a functional builder pattern to set up the application. To communicate between the .NET application and the web app there is a Javascript request interface. The web site can be hosted as .NET resource, of course alternatively via HTTP(s):// or file://.

WebWindowNetCore > version 11.0.0 is redesigned and has dropped included HTTP Server

> WebViewNetCore.Linux and WebViewNetCore.Windows are now obsolete!

## Features

WebWindowNetCore includes following features:
* Is built on .NET 8
* Functional approach with a builder pattern
* (almost) the same setup for Windows and Linux version
* Uses WebView2 on Windows and WebKitGtk-6.0 on Linux
* Can serve the web site via .NET resources (single file approach)
* Optional save and restore of window bounds
* Has an integrated event sink mechanismen, so you can retrieve javascript events from the .NET app
* Has an integrated Javascript WebView interface to serve requests from javascript to .NET app
* You can expand the Gtk Window (on Linux) with a custom header bar
* You can alternatively disable the Windows titlebar and borders, and you can build a title bar in HTML with standard Windows logic for closing, maximizing, restoring resizing, snap to dock, ...

Functional approach with webview builder:

```cs
new WebView()
    .AppId("de.uriegel.test")
    .InitialBounds(1200, 800)
    .Title("Web Window Net Core 👍")
    .ResourceIcon("icon")
    .ResourceScheme()
    .SaveBounds()
    .DefaultContextMenuDisabled()
    .AddRequest<Input, Contact>("cmd1", GetContact)
    .AddRequest<Input2, Contact2>("cmd2", GetContact2)
#if DEBUG    
    .DevTools()
#endif
    .Url("res://webroot/index.html")
    .CanClose(() => true)
    .OnStarted(action => action.ExecuteJavascript ("console.log('app started now ')"))
    .Run();
```
For further description and tutorial see [WebWindowNetCore description](https://github.com/uriegel/WebWindowNetCore)
