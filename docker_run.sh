docker run --rm  --name minimal-nuget-server -p 4356:4356 -it rolfwessels/minimal-nuget-server:latest 

# To debug this i run the base container in it mode
# docker run --rm  --name aspnetcore-build  -it microsoft/aspnetcore-build:2.0.0-stretch bash
# > git clone https://github.com/rolfwessels/MinimalNugetServer.git
# cd /MinimalNugetServer
# dotnet restore
# dotnet publish --output /dotnetapp
# cd /dotnetapp
# cp /MinimalNugetServer/MinimalNugetServer/configuration.json /dotnetapp
# dotnet MinimalNugetServer.dll
