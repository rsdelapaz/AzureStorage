FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 8080

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY Azurebrains.ValetKey.WebAPI/Azurebrains.ValetKey.WebAPI.csproj Azurebrains.ValetKey.WebAPI/
RUN dotnet restore
COPY . .
WORKDIR /src/Azurebrains.ValetKey.WebAPI
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Azurebrains.ValetKey.WebAPI.dll"]
