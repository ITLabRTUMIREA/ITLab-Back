FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
RUN apt-get update && apt-get install -y libgdiplus libc6-dev && ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY LaboratoryWorkControl.sln ./
COPY BackEnd/BackEnd.csproj BackEnd/
COPY Models/Models.csproj Models/
COPY Extensions/Extensions.csproj Extensions/
COPY Models.PublicAPI/Models.PublicAPI.csproj Models.PublicAPI/
COPY ApiSchemaCLI/ApiSchemaCLI.csproj ApiSchemaCLI/
COPY WebApp.Configure/WebApp.Configure.csproj WebApp.Configure/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/BackEnd
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM rtuitlab/itlab-front-master AS front

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=front /usr/share/nginx/html ./wwwroot/
ENTRYPOINT ["dotnet", "BackEnd.dll"]
