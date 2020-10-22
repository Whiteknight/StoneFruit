. ./_allProjects.sh
for project in ${all_projects[@]}; do
    dotnet restore Src/$project/$project.csproj
    dotnet build Src/$project/$project.csproj --configuration Release --no-restore
    dotnet pack Src/$project/$project.csproj --no-build --no-restore
done


dotnet build -f netstandard2.0 Src/StoneFruit.Containers.Unity/StoneFruit.Containers.Unity.csproj --configuration Release
dotnet pack Src/StoneFruit.Containers.Unity/StoneFruit.Containers.Unity.csproj --configuration Release --no-build --no-restore

