FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY LaboratoryWorkControl.sln ./
COPY BackEnd/BackEnd.csproj BackEnd/
COPY Models/Models.csproj Models/
COPY Extensions/Extensions.csproj Extensions/
COPY Models.PublicAPI/Models.PublicAPI.csproj Models.PublicAPI/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/BackEnd
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BackEnd.dll"]
