FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY ITLab-Back/itlab-back-build .
COPY ITLab-Front/itlab-front-build ./wwwroot/
ENTRYPOINT ["dotnet", "BackEnd.dll"]