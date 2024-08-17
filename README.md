# WebWindowNetCore
A .NET Webview Application for Windows and Linux similar to Electron. It uses a functional builder pattern to set up the application. It has an integrated Asp.NET (Kestrel) server which can be used to host the app and to communicate between the .NET application and the web app. The web site can be hosted as .NET resource, of course alternatively via HTTP(s):// or file://.

![Sample WebView app](readme/sampleapp.png)
Sample WebWindowNetCore app

WebWindowNetCore > version 10.0.0 is completely redesigned and programmed in F#, so that it is "F# friendly". Of course C# is supported as well. Unlike the older versions, there is no other Nuget packe required other than this. 

> WebViewNetCore.Linux and WebViewNetCore.Windows are now obsolete!

