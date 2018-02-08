# MinimalNugetServer

## Api

MinimalNugetServer uses the Nuget v2 API to support listing/installing/updating/pushing nuget packages.

Supported clients are Visual Studio 2017, Rider, and the .NET Core 2 CLI.

No API key is required to push. Use the following source URL to push:

`http://<host>/v2/push/<package-name-with-version.nupkg>`

## Building docker environment

Add `COMPOSE_CONVERT_WINDOWS_PATHS=1` to your environment variables, then create a folder at `C:\nuget_server_local\`.

Then run:

`cd .\MinimalNugetServer\`

`docker-compose build`

## Starting docker environment

`docker-compose up -d`