FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY ITLab-Back/itlab-identity-build .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]