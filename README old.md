# WebWindowNetCore

# Table of contents
1. [Introduction](#introduction)
2. [Some paragraph](#paragraph1)
    1. [Sub paragraph](#subparagraph1)
3. [Another paragraph](#paragraph2)

## This is the introduction <a name="introduction"></a>
Some introduction text, formatted in heading 2 style

sdfdsaf
 
 asd
f

as

fgasd

gdas

gadsg

ads

g

d s
fgsdf
g sdfg

sd

sd fgdsf

sdg
df


sd fgs

gs

dgs

d
f


## Some paragraph <a name="paragraph1"></a>
The first paragraph text

sdfdsaf
 
 asd
f

as

fgasd

gdas

gadsg

ads

g

d s
fgsdf
g sdfg

sd

sd fgdsf

sdg
df


sd fgs

gs

dgs

d
f


### Sub paragraph <a name="subparagraph1"></a>
This is a sub paragraph, formatted in heading 3 style

sdfdsaf
 
 asd
f

as

fgasd

gdas

gadsg

ads

g

d s
fgsdf
g sdfg

sd

sd fgdsf

sdg
df


sd fgs

gs

dgs

d
f

sdfdsaf
 
 asd
f

as

fgasd

gdas

gadsg

ads

g

d s
fgsdf
g sdfg

sd

sd fgdsf

sdg
df


sd fgs

gs

dgs

d
f


## Another paragraph <a name="paragraph2"></a>
The second paragraph text


# Table of Contents
1. [Example](#example)
2. [Example2](#example2)
3. [Third Example](#third-example)
4. [Fourth Example](#fourth-examplehttpwwwfourthexamplecom)


## Example

yxsgfdfs 
df
g dsfg
 
 sdfh
  f
  h
   fdgh
    fdgh
    fdg
    h
## Example2

f hgfgdh 
fdgh
 fdg
 h 
 dfhg
  df
  h
   df
   h
   dfg h
   fdg
   h d
## Third Example
d fghdfgh 
dfhg
 
 dfgh
  df
  hg 
  dfh
   d
   fhg
    df
    hg
    df
## [Fourth Example](http://www.fourthexample.com) 

df ghj
df
gh 
df
h 
dfgh
dfg

## Prerequisites 
### Linux

Abhängig von 
Sometimes it is necessary to install libgtk-4-dev ???
```
sudo apt install libgtk-4-dev 
```

### Windows

WebView2 runtime (installed on Window 10 and 11, but not on Windows Server)

install it:

## next

```cs
{
    var test = new Class(239)
    var erg = test.Run()
}
```

```fs
{
    let test = new Class(239)
    let erg = test ()
        |> hallo
}
```

## Notes for Developers

### Developing for Windows
In the project file ```WebWindowNetCoe.fsproj``` replace the following line

```<TargetFrameworks>net8.0;net8.0-windows</TargetFrameworks>```

with 

```<TargetFrameworks>net8.0-windows</TargetFrameworks>```

Don't forget to replace it to the original value before publishing a new nuget version

## new

* WebWindowNetCore.Window and .Linux to this project
* Publish WebwindowNetCore only from Windows
* Test lionux version in Linux with if Windows Windows code and Windows.Forms.dlls



 

## Functional builder setup in the C# application
There is a builder pattern to setup the application (```WebView.Create```):


```
WebView
    .Create()
    .InitialBounds(600, 800)
    .Title("WebView Test")
    .ResourceIcon("icon")
    .SaveBounds()
    .Url($"file://{Directory.GetCurrentDirectory()}/webroot/index.html")
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run();    
```
### Setup integrated kestrel server

The integrated Asp.NET kestrel server also uses a builder pattern (```ConfigureHttp```)

```
WebView
    .Create()
    .InitialBounds(600, 800)
    .Title("WebView Test")
    .ResourceIcon("icon")
    .SaveBounds()
    .ConfigureHttp(http => http
        .ResourceWebroot("webroot", "/web")
        .UseJsonPost<Cmd1Param, Cmd1Result>("request/cmd1", JsonRequest1)
        .UseJsonPost<Cmd2Param, Cmd2Result>("request/cmd2", JsonRequest2)
        .UseSse("sse/test", sseEventSource)
        .Build())
#if DEBUG            
    .DebuggingEnabled()
#endif            
    .Build()
    .Run("de.uriegel.Commander");    
```
The default (local) port is 20000. It can be adapted.

### Serving web app files from resource

It is possible to serve the web app via the integrated kestrel server, when the web app files are included as C# resources. You have to specify the resource root, under which the resource files are saved.

Please refer to the sample App

### How to setup .NET project:

```
<PropertyGroup>
    <IsWindows Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::Windows)))' == 'true'">true</IsWindows> 
		<IsOSX Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::OSX)))' == 'true'">true</IsOSX> 
	<IsLinux Condition="'$([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform($([System.Runtime.InteropServices.OSPlatform]::Linux)))' == 'true'">true</IsLinux>    
</PropertyGroup>

<PropertyGroup Condition="'$(IsWindows)'=='true'">
    <OutputType>WinExe</OutputType>
    <TargetFramework>net6.0-windows</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup> 

<PropertyGroup Condition="'$(IsLinux)'=='true'">
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
</PropertyGroup>

<ItemGroup Condition="'$(IsWindows)'=='true'">
    <PackageReference Include="WebWindowNetCore.Windows" Version="*.*.*" />
</ItemGroup> 

<ItemGroup Condition="'$(IsLinux)'=='true'">
    <PackageReference Include="WebWindowNetCore.Linux" Version="*.*.*" />
</ItemGroup> 

```

