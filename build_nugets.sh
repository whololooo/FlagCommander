#!/bin/zsh

# This script builds the NuGet packages for the project.
root_dir=$(pwd)

cd $root_dir/src/FlagCommander
dotnet build -c Release -f net9.0
dotnet build -c Release -f net8.0
dotnet build -c Release -f net6.0
nugetArgs="FlagCommander.nuspec -OutputFileNamesWithoutVersion -OutputDirectory $root_dir/artifacts/"
nuget pack $nugetArgs || mono /usr/local/bin/nuget.exe pack $nugetArgs
cd $root_dir

cd $root_dir/src/FlagCommanderUI
dotnet build -c Release -f net9.0
dotnet build -c Release -f net8.0
nugetArgs="FlagCommanderUI.nuspec -OutputFileNamesWithoutVersion -OutputDirectory $root_dir/artifacts/"
nuget pack $nugetArgs || mono /usr/local/bin/nuget.exe pack $nugetArgs
cd $root_dir

cd $root_dir/src/FlagCommanderUINet6
dotnet build -c Release -f net6.0
nugetArgs="FlagCommanderUINet6.nuspec -OutputFileNamesWithoutVersion -OutputDirectory $root_dir/artifacts/"
nuget pack $nugetArgs || mono /usr/local/bin/nuget.exe pack $nugetArgs
cd $root_dir

# TODO push nugets to nuget.org