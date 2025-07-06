#!/bin/bash

# Path to the OpenCover.Console.exe executable
# TODO: see if we can find a way to make this a relative path for a more repeatable process
OPENCOVER_EXE=$HOMEPATH/.nuget/packages/opencover/4.7.922/tools/OpenCover.Console.exe

# URL to Sonarqube. If nginx is working we can use the first. "if"
SONARQUBE_URL=http://localhost:9000

dotnet sonarscanner begin \
    -d:sonar.token="$SQ_STONEFRUIT_KEY" \
    -d:sonar.host.url="$SONARQUBE_URL" \
    -k:StoneFruit \
    -d:"sonar.cs.opencover.reportsPaths=coverage.xml"

dotnet build Src/StoneFruit.sln

$OPENCOVER_EXE \
    -target:"c:\Program Files\dotnet\dotnet.exe" \
    -targetargs:"test Src/StoneFruit.sln" \
    -output:"coverage.xml" \
    -oldstyle \
    -register:user

dotnet sonarscanner end -d:sonar.token="$SQ_STONEFRUIT_KEY"
rm coverage.xml