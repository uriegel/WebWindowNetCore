# WebWindowNetCore
A .NET 8 Webview Application for Windows and Linux similar to Electron. It uses a functional builder pattern to set up the application. It has an integrated Asp.NET (Kestrel) server which can be used to host the app and to communicate between the .NET application and the web app. The web site can be hosted as .NET resource, of course alternatively via HTTP(s):// or file://.

![Sample WebView app](readme/sampleapp.png)

Sample WebWindowNetCore app

WebWindowNetCore > version 10.0.0 is completely redesigned and programmed in F#, so that it is "F# friendly". Of course C# is supported as well. Unlike the older versions, there is no other Nuget packe required other than this. 

> WebViewNetCore.Linux and WebViewNetCore.Windows are now obsolete!

# Table of contents
1. [Introduction](#features)
2. [Setup](#setup)
    1. [Prerequisites for Windows](#prewindows)
    2. [Prerequisites for Linux](#prelinux)
   


## Features <a name="features"></a>

WebWindowNetCore includes following features:
* Is built on .NET 8
* Functional approach with a builder pattern
* (almost) the same setup for Windows and Linux version
* Uses WebView2 on Windows and WebKitGtk-6.0 on Linux
* Can serve the web site via .NET resources (single file approach)
* Optional save and restore of window bounds
* Has an integrated event sink mechanismen, so you can retrieve javascript events from the .NET app
* Has an integrated .NET Kestrel Server (optional) to serve requests from .NET app to javascript
* You can expand the Gtk Window (on Linux) with a custom header bar
* You can alternatively disable the Windows titlebar and borders, and you can build a title bar in HTML with standard Windows logic for closing, maximizing, restoring resizing, snap to dock, ...


![custom titlebar](readme/customTitlebar.png) 

Sample of a Windows App with custom titlebar



## Setup <a name="setup"></a>

### Prerequisites for Windows <a name="prewindows"></a>

On Windows 10 or Windows 11 WebView2 is already installed, you don't have to do anything.

If you want to run the WebView app on a Windows Server, you have to install the necessary WebView2 runtime. Please consult the relevant web site from Microsoft.

### Prerequisites for Linux <a name="prelinux"></a>

On modern Linux like Ubuntu 24.04 WebWindowNetCore app will run out of the box (if you create a full contained single file exe), otherwise you have to install the necessary dotnet runtime.

On older/other Linux systems perhaps you have to install one of the following packages in order to make the app runnable, like

```
sudo apt install libgtk-4-dev
sudo apt install libadwaita-1-dev
sudo apt install libwebkitgtk-6.0-dev
```

