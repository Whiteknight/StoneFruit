#!/bin/bash

fileversion=$(grep "<Version>" "Src/StoneFruit/StoneFruit.csproj" | sed 's/.*<Version>\(.*\)<\/Version>/\1/')
version=${1:-$fileversion} 
dotnet nuget push Src/StoneFruit/bin/Release/StoneFruit.$1.nupkg --source https://www.nuget.org/api/v2/package
dotnet nuget push Src/StoneFruit/bin/Release/StoneFruit.$1.snupkg --source https://www.nuget.org/api/v2/package
