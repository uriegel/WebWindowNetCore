# WebWindowNetCore
A .NET 8 Webview Application for Windows and Linux similar to Electron. It uses a functional builder pattern to set up the application. It has an integrated Asp.NET (Kestrel) server which can be used to host the app and to communicate between the .NET application and the web app. The web site can be hosted as .NET resource, of course alternatively via HTTP(s):// or file://.

Sample WebWindowNetCore app:
![Sample WebView app](readme/sampleapp.png)

WebWindowNetCore > version 10.0.0 is completely redesigned and programmed in F#, so that it is "F# friendly". Of course C# is supported as well. Unlike the older versions, there is no other Nuget packet required other than this. 

> WebViewNetCore.Linux and WebViewNetCore.Windows are now obsolete!

# Table of contents
1. [Introduction](#features)
2. [Setup](#setup)
    1. [Prerequisites for Windows](#prewindows)
    2. [Prerequisites for Linux](#prelinux)
3. [Hello World (a minimal web view app)](#helloworld)
    1. [Adaptions for debug and build integration in visual studio code](#adaptionHelloWorld)
4. [Features of WebViewBuilder](#features)
    1. [Creating WebViewBuilder and running app](#featuresCreating)


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
Sample of a Windows App with custom titlebar:
![custom titlebar](readme/customTitlebar.png) 

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
## Hello World (a minimal web view app) <a name="helloworld"></a>

In this tutoriual I am using Visula Studio Code, but of course you can also use Visual Studio, but only on Windows.

* Start Visual Studio Code and navigate ot a newly created project folder, in this case ```~/projects/HelloWorld```
* Open a terminal window in code and type ```dotnet new console```.
* Press ```F5``` to run the newly created project and to see that it runs
* Set up the project file so that it looks like this: 

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsWindows Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::Windows)))' == 'true'">true</IsWindows> 
		<IsOSX Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::OSX)))' == 'true'">true</IsOSX> 
		<IsLinux Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::Linux)))' == 'true'">true</IsLinux>  
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <PropertyGroup Condition="'$(IsWindows)'=='true'">
    <OutputType>WinExe</OutputType>
    <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
    <TargetFramework>net8.0-windows</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <SelfContained>false</SelfContained>
  </PropertyGroup> 

  <PropertyGroup Condition="'$(IsLinux)'=='true'">
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
    <SelfContained>true</SelfContained>
  </PropertyGroup> 

  <PropertyGroup Condition="'$(IsWindows)'=='true'">
    <DefineConstants>Windows</DefineConstants>
  </PropertyGroup>

  <PropertyGroup Condition="'$(IsLinux)'=='true'">
    <DefineConstants>Linux</DefineConstants>
  </PropertyGroup>

</Project>
```
In the first property group we declare ```IsWindows``` and ```IsLinux``` conditions based on the platform.

In the second and third property group we make platform dependant changes, like ```OutputType``` ```WinExe``` on Windows and ```Exe``` on Linux. We also set up the correct runtime ans target identifiers. 

In the two last property groups we define constants ```Windows``` and ```Linux``` based on the current platform which you can use to in code with preprocessor conditions:

```
#ifdef Linux
    ...
#endif
```
Now we can import the necessary nuget package WebWindowNetCore (version 10 or higher!) by typing the following in the terminal window:

```
dotnet add package WebWindowNetCore --version 10.0.0-beta-9
```

For a minimal program replace all from the file "Program.cs" with:


```cs
using WebWindowNetCore;

new WebView()
    .Url("https://google.de")
    .Run();
```

Now type ```dotnet run``` in the terminal and the web view app is starting and you see something like this:

![hello world app](readme/helloworld.png)

Congratulations! Your first web view app is running!

### Adaptions for debug and build integration in visual studio code <a name="adaptionHelloWorld"></a>

When you press ```F5```, the old terminal program is running, not the newly build web view app. This is because you has changed the target and runtime identifier. In order to debug the app in visul studio code you should add two files in a folder ```.vscode```, if they are not already present:

![.vscode folder](readme/vscode.png)

The content of ```tasks.json``` shoud look like this:

```
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/HelloWorld.csproj",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary;ForceNoAlign"
            ],
            "problemMatcher": "$msCompile"
        }
    ]
}        
```

while ```launch.json``` should lkook like

```
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (Linux)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/bin/Debug/net8.0/linux-x64/HelloWorld.dll",
            "args": [],
            "cwd": "${workspaceFolder}",
            "console": "internalConsole",
            "stopAtEntry": false
        }, {
            "name": ".NET Core Launch (Windows)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}bin/Debug/net8.0-windows/win-x64/HelloWorld.dll",
            "args": [],
            "cwd": "${workspaceFolder}",
            "console": "internalConsole",
            "stopAtEntry": false
        }
    ]
}
```

Now you can choose your platform (Linux or Windows) in the debugger tab of the sidebar in Visual Studio Code and build and debug your app.

## Features of WebViewBuilder <a name="features"></a>

### Creating WebViewBuilder and running app <a name="featuresCreating"></a>







new WebView()

.Run() starts the WebView window and runs the application until the window is closed

### FromResource
### Title
### appid
### SaveBounds
### ResourceScheme
### ResourceIcon