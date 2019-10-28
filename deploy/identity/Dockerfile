FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
COPY ITLab-Back/itlab-identity-build .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]