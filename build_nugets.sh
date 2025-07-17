#!/bin/bash

# This script builds the NuGet packages for the project.
root_dir=$(pwd)

cd $root_dir/src/FlagCommander
dotnet build -c Release
nuget pack FlagCommander.nuspec -OutputFileNamesWithoutVersion -OutputDirectory $root_dir/artifacts/
cd $root_dir

cd $root_dir/src/FlagCommanderUI
dotnet build -c Release
nuget pack FlagCommanderUI.nuspec -OutputFileNamesWithoutVersion -OutputDirectory $root_dir/artifacts/
cd $root_dir

cd $root_dir/src/FlagCommanderUINet6
dotnet build -c Release
nuget pack FlagCommanderUINet6.nuspec -OutputFileNamesWithoutVersion -OutputDirectory $root_dir/artifacts/ 
cd $root_dir

# TODO push nugets to nuget.org