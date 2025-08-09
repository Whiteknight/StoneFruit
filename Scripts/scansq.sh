#!/bin/bash

# PREREQUISITE: Install Coverlet 
#   dotnet tool install --global coverlet.console

# URL to Sonarqube. If nginx is working we can use the first. "if"
SONARQUBE_URL=http://localhost:9000

rm -rf Src/StoneFruit.SpecTests/TestResults
rm -rf Src/StoneFruit.Tests/TestResults

dotnet sonarscanner begin \
    -d:sonar.token="$SQ_STONEFRUIT_KEY" \
    -d:sonar.host.url="$SONARQUBE_URL" \
    -k:StoneFruit \
    -d:"sonar.cs.opencover.reportsPaths=**/coverage.opencover.xml"

dotnet build --no-incremental Src/StoneFruit.sln

#dotnet test --collect:"XPlat Code Coverage;Format=opencover"  Src/StoneFruit.sln
dotnet test --collect:"XPlat Code Coverage;Format=opencover"  Src/StoneFruit.Tests/StoneFruit.Tests.csproj
dotnet test --collect:"XPlat Code Coverage;Format=opencover"  Src/StoneFruit.SpecTests/StoneFruit.SpecTests.csproj

dotnet sonarscanner end -d:sonar.token="$SQ_STONEFRUIT_KEY"
