# Build stage
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

# Publish stage
FROM build AS publish
RUN dotnet publish "Planty.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS final
WORKDIR /app

# Install PostgreSQL client tools for migration checks
RUN apt-get update && apt-get install -y \
    postgresql-client \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Copy entrypoint and helper scripts
COPY scripts/ /app/scripts/

# Create data directory for uploads
RUN mkdir -p /app/data/uploads && chmod -R 755 /app/data

EXPOSE 5000

# Set environment defaults (can be overridden)
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Make entrypoint script executable
RUN chmod +x /app/scripts/entrypoint.sh

# Note: ENTRYPOINT and CMD are defined in docker-compose.yml
# This allows for easier override in different environments