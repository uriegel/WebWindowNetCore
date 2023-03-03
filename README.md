# WebWindowNetCore
## New concept

Only this package including 3 assemblies:
* WebWindowNetCore
* WindowsWebWindow
* LinuxWebWindow

WindowsWebWindow in Directory 
```
runtimes
    \win10-x64
        \lib\WebWindow
        \native\WebViewLoader.dll
    \linux-x64
        \lib\WebWindow
```
## Test

Small sample with Gtk Window ans Windows Forms,
publish under Version 0.0.9

## Deprecated
WebWindowNetCore is the base Nuget packege, you need one more Nuget packages for every to supporting Operating system:

| Windows  | Linux  |
|---|---|
| WebWindowNetCore.Window  |  WebWindowNetCore.Linux |

How to setup .NET project:

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
    <PackageReference Include="WebWindowNetCore.Windows" Version="0.0.1-alpha.2" />
</ItemGroup> 

<ItemGroup Condition="'$(IsLinux)'=='true'">
    <PackageReference Include="WebWindowNetCore.Linux" Version="0.0.1-alpha.2" />
</ItemGroup> 

```

## Loading application icon
In order to attach an application icon, an icon has to be inserted as resource. Look at README.md in the platform specific package (WebWindowNetCore.Windows, WebWindowNetCore.Linux).