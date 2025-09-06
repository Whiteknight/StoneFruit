#!/bin/bash

fileversion=$(grep "<Version>" "Src/StoneFruit/StoneFruit.csproj" | sed 's/.*<Version>\(.*\)<\/Version>/\1/')
version=${1:-$fileversion} 
dotnet nuget push Src/StoneFruit/bin/Release/StoneFruit.$version.nupkg --source https://www.nuget.org/api/v2/package
dotnet nuget push Src/StoneFruit/bin/Release/StoneFruit.$version.snupkg --source https://www.nuget.org/api/v2/package
