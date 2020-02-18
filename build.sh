dotnet build -f netstandard2.0 Src/StoneFruit/StoneFruit.csproj --configuration Release
dotnet pack StoneFruit.csproj --configuration Release --no-build --no-restore

dotnet build -f netstandard2.0 Src/StoneFruit.StructureMap/StoneFruit.StructureMap.csproj --configuration Release
dotnet pack Src/StoneFruit.StructureMap/StoneFruitStructureMap.csproj --configuration Release --no-build --no-restore
