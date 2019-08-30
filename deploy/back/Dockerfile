FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY ITLab-Back/itlab-back-develop .
COPY ITLab-Front/itlab-front-develop ./wwwroot/
ENTRYPOINT ["dotnet", "BackEnd.dll"]