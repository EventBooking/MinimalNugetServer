FROM microsoft/aspnetcore-build:2.0.0-stretch AS publish

COPY . /src
WORKDIR /src/MinimalNugetServer

RUN dotnet restore
RUN dotnet publish --output /src/out

FROM microsoft/aspnetcore:2.0.0-stretch

ENV ASPNETCORE_URLS http://*:4356
ENV ASPNETCORE_SERVER_URL http://*:4356
ENV ASPNETCORE_ENVIRONMENT "Production"

WORKDIR /dotnetapp
COPY --from=publish /src/out .
COPY --from=publish /src/MinimalNugetServer/configuration.json .
COPY --from=publish /src/docker_fix_config_file.sh .
RUN sh docker_fix_config_file.sh  && \
    cat configuration.json && \
    mkdir /packages

EXPOSE 4356
VOLUME ["/packages"]

ENTRYPOINT ["dotnet", "MinimalNugetServer.dll"]