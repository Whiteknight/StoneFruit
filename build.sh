#!/bin/bash

dotnet restore Src/StoneFruit/StoneFruit.csproj
dotnet build Src/StoneFruit/StoneFruit.csproj --configuration Release --no-restore
dotnet pack Src/StoneFruit/StoneFruit.csproj --no-build --no-restore
