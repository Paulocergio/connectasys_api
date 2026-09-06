FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/API/connectasys_api.API.csproj src/API/
COPY src/Core/connectasys_api.Core.csproj src/Core/
COPY src/Infrastructure/connectasys_api.Infrastructure.csproj src/Infrastructure/
RUN dotnet restore src/API/connectasys_api.API.csproj

COPY src/ src/
RUN dotnet publish src/API/connectasys_api.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "connectasys_api.API.dll"]
