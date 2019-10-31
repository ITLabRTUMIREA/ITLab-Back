FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

RUN apt-get update
RUN apt-get install libc6-dev -y
RUN apt-get install libgdiplus -y

WORKDIR /app

COPY ITLab-Back/itlab-back-build .
COPY ITLab-Front/itlab-front-build ./wwwroot/
ENTRYPOINT ["dotnet", "BackEnd.dll"]
