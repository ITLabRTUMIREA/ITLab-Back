FROM microsoft/dotnet:2.1-aspnetcore-runtime

RUN apt install libc6-dev 
RUN apt install libgdiplus

WORKDIR /app

COPY ITLab-Back/itlab-back-build .
COPY ITLab-Front/itlab-front-build ./wwwroot/
ENTRYPOINT ["dotnet", "BackEnd.dll"]
