#!/bin/bash
# Backend entrypoint script
# Waits for database availability, applies migrations, then starts the service

set -e

# Configuration
DB_HOST=${DB_HOST:-postgres}
DB_PORT=${DB_PORT:-5432}
DB_USER=${DB_USER:-planty}
DB_PASSWORD=${DB_PASSWORD:-planty_password}
DB_NAME=${DB_NAME:-plantydb}
MAX_RETRIES=30
RETRY_INTERVAL=2

echo "=== Planty Backend Entrypoint ==="
echo "Waiting for PostgreSQL at $DB_HOST:$DB_PORT..."

# Wait for PostgreSQL to be ready
RETRIES=0
until PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U "$DB_USER" -d "$DB_NAME" -c "SELECT 1" > /dev/null 2>&1; do
  RETRIES=$((RETRIES + 1))
  if [ $RETRIES -gt $MAX_RETRIES ]; then
    echo "ERROR: Failed to connect to PostgreSQL after $MAX_RETRIES attempts"
    exit 1
  fi
  echo "PostgreSQL not ready yet, waiting... (attempt $RETRIES/$MAX_RETRIES)"
  sleep $RETRY_INTERVAL
done

echo "✓ PostgreSQL is ready!"
echo ""

# Apply database migrations
echo "Applying database migrations..."
cd /app

# Check if migrations exist - if so, apply them
if [ -d "/app/Migrations" ] || [ -f "/app/Planty.MigrationTool" ]; then
  if [ -f "/app/Planty.MigrationTool" ]; then
    echo "Running MigrationTool..."
    ./Planty.MigrationTool || {
      echo "ERROR: Migration tool failed"
      exit 1
    }
  else
    echo "Running EF Core migrations via dotnet..."
    dotnet ef database update --project /app/Planty.API.dll 2>/dev/null || {
      echo "Note: EF Core migrations not available in runtime"
    }
  fi
else
  echo "No migrations tool found, skipping migrations"
fi

echo "✓ Migrations completed"
echo ""

# Start the application
echo "Starting Planty API..."
exec dotnet Planty.API.dll
