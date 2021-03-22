FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY Survival Game Backend Server/bin/Release/netcoreapp3.1/publish
WORKDIR /publish
ENTRYPOINT ["dotnet", "Survival Game Backend Server.dll"]