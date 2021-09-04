
. ./_allProjects.sh
for project in ${all_projects[@]}; do
    dotnet nuget push Src/$project/bin/Release/$project.$1.nupkg --source https://www.nuget.org/api/v2/package
    dotnet nuget push Src/$project/bin/Release/$project.$1.snupkg --source https://www.nuget.org/api/v2/package
done
