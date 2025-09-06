FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/PlantApp.API/PlantApp.API.csproj", "src/PlantApp.API/"]
COPY ["src/PlantApp.Application/PlantApp.Application.csproj", "src/PlantApp.Application/"]
COPY ["src/PlantApp.Contracts/PlantApp.Contracts.csproj", "src/PlantApp.Contracts/"]
COPY ["src/PlantApp.Domain/PlantApp.Domain.csproj", "src/PlantApp.Domain/"]
COPY ["src/PlantApp.Infrastructure/PlantApp.Infrastructure.csproj", "src/PlantApp.Infrastructure/"]

RUN dotnet restore "src/PlantApp.API/PlantApp.API.csproj"
COPY . .
WORKDIR "/src/src/PlantApp.API"
RUN dotnet build "PlantApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlantApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create data directory for SQLite database
RUN mkdir -p /app/data && chown -R $APP_UID:$APP_UID /app/data

VOLUME ["/app/data"]
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PlantApp.API.dll"]