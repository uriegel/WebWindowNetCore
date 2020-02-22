# WebWindowNetCore
## Build nuget package
```dotnet publish -c Release```
## Test nuget package
In test project, run 
```dotnet add package WebWindowNetCore -s file:///media/speicher/projekte/WebWindowNetCore/WebWindowNetCore/bin/Release/```

## Clean nuget cache
```dotnet nuget locals all --clear```

## Debug Windows Version
Do  N O T  us ```dotnet``` as Debugger (Ionide), use F5 and debug .exe!