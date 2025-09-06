FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Planty.API/Planty.API.csproj", "src/Planty.API/"]
COPY ["src/Planty.Application/Planty.Application.csproj", "src/Planty.Application/"]
COPY ["src/Planty.Contracts/Planty.Contracts.csproj", "src/Planty.Contracts/"]
COPY ["src/Planty.Domain/Planty.Domain.csproj", "src/Planty.Domain/"]
COPY ["src/Planty.Infrastructure/Planty.Infrastructure.csproj", "src/Planty.Infrastructure/"]

RUN dotnet restore "src/Planty.API/Planty.API.csproj"
COPY . .
WORKDIR "/src/src/Planty.API"
RUN dotnet build "Planty.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Planty.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create data directory for SQLite database
RUN mkdir -p /app/data && chown -R $APP_UID:$APP_UID /app/data

VOLUME ["/app/data"]
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Planty.API.dll"]