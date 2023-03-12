# WebWindowNetCore

A Webview Application for Windows and Linux similar to Electron. It uses a functional builder pattern to set up the application. It has an integrted Asp.NET (Kestrel) server which can be used to host the app and to cummunicate between the C# application and the web app.


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
    <PackageReference Include="WebWindowNetCore.Windows" Version="1.0.0.1" />
</ItemGroup> 

<ItemGroup Condition="'$(IsLinux)'=='true'">
    <PackageReference Include="WebWindowNetCore.Linux" Version="1.0.0.1" />
</ItemGroup> 

```

