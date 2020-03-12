dotnet build -f netstandard2.0 Src/StoneFruit/StoneFruit.csproj --configuration Release
dotnet pack StoneFruit.csproj --configuration Release --no-build --no-restore

dotnet build -f netstandard2.0 Src/StoneFruit.Containers.StructureMap/StoneFruit.Containers.StructureMap.csproj --configuration Release
dotnet pack Src/StoneFruit.Containers.StructureMap/StoneFruit.Containers.StructureMap.csproj --configuration Release --no-build --no-restore

dotnet build -f netstandard2.0 Src/StoneFruit.Containers.Lamar/StoneFruit.Containers.Lamar.csproj --configuration Release
dotnet pack Src/StoneFruit.Containers.Lamar/StoneFruit.Containers.Lamar.csproj --configuration Release --no-build --no-restore

dotnet build -f netstandard2.0 Src/StoneFruit.Containers.Ninject/StoneFruit.Containers.Ninject.csproj --configuration Release
dotnet pack Src/StoneFruit.Containers.Ninject/StoneFruit.Containers.Ninject.csproj --configuration Release --no-build --no-restore

