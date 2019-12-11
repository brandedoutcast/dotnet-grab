# ✨ grab [dotnet-grab]
.NET core global tool to download nuget packages in to a directory without any project

## Why
Useful to manage dependencies for C# / F# script files that can be executed via REPL

## Install
The `dotnet-grab` nuget package is [published to nuget.org](https://www.nuget.org/packages/dotnet-grab/)

You can get the tool by running this command

`$ dotnet tool install -g dotnet-grab`

## Usage

    Usage: grab [packages...]

    packages:
        list of packages names (@ optional versions) to download

    Ex:
        grab newtonsoft.json newtonsoft.json@6.0.7 entityframework@6.2.0

        newtonsoft.json @ 12.0.3 - downloaded
        newtonsoft.json @ 6.0.7 - downloaded
        entityframework @ 6.2.0 - downloaded

    packages are saved under 'packages' in current directory