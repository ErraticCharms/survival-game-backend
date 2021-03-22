FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

ENTRYPOINT [ "dotnet", "Survival Game Backend Server.dll" ]