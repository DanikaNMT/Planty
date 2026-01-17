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

# Apply migrations using the MigrationTool
if [ -f "/app/Planty.MigrationTool" ]; then
  echo "Running MigrationTool..."
  chmod +x /app/Planty.MigrationTool
  ./Planty.MigrationTool || {
    echo "ERROR: Migration tool failed"
    exit 1
  }
else
  echo "ERROR: Planty.MigrationTool not found at /app/Planty.MigrationTool"
  exit 1
fi

echo "✓ Migrations completed"
echo ""

# Start the application
echo "Starting Planty API..."
exec dotnet Planty.API.dll
