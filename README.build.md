# How to build and debug

## Debugging

### Linux
In WebWindowNetCore.csproj change

```
<PropertyGroup>
    <TargetFrameworks>net10.0;net10.0-windows</TargetFrameworks>
    <!-- <TargetFrameworks>net10.0</TargetFrameworks> -->
    <!-- <TargetFrameworks>net10.0-windows</TargetFrameworks> -->
</PropertyGroup>
  ```

  to 

```
<PropertyGroup>
    <!-- <TargetFrameworks>net10.0;net10.0-windows</TargetFrameworks> -->
    <TargetFrameworks>net10.0</TargetFrameworks>
    <!-- <TargetFrameworks>net10.0-windows</TargetFrameworks> -->
</PropertyGroup>
  ```

### Windows
In WebWindowNetCore.csproj change

```
<PropertyGroup>
    <TargetFrameworks>net10.0;net10.0-windows</TargetFrameworks>
    <!-- <TargetFrameworks>net10.0</TargetFrameworks> -->
    <!-- <TargetFrameworks>net10.0-windows</TargetFrameworks> -->
</PropertyGroup>
  ```

  to 

```
<PropertyGroup>
    <!-- <TargetFrameworks>net10.0;net10.0-windows</TargetFrameworks> -->
    <!-- <TargetFrameworks>net10.0</TargetFrameworks> -->
    <TargetFrameworks>net10.0-windows</TargetFrameworks>
</PropertyGroup>
  ```

## Packaging and Deploying

This is only possible in Windows! Change WebWindowNetCore.csproj back to

```
<PropertyGroup>
    <TargetFrameworks>net10.0;net10.0-windows</TargetFrameworks>
    <!-- <TargetFrameworks>net10.0</TargetFrameworks> -->
    <!-- <TargetFrameworks>net10.0-windows</TargetFrameworks> -->
</PropertyGroup>
  ```

Now run the command

```
cd WebWindowNetCore
dotnet build -c Release
```