sed -i -E "s/<Version>[0-9]+.[0-9]+.[0-9]+/<Version>$1/" \
    Src/StoneFruit/StoneFruit.csproj \
    Src/StoneFruit.Containers.Lamar/StoneFruit.Containers.Lamar.csproj \
    Src/StoneFruit.Containers.Ninject/StoneFruit.Containers.Ninject.csproj \
    Src/StoneFruit.Containers.StructureMap/StoneFruit.Containers.StructureMap.csproj
    