#!/bin/bash
# Migration script: SQLite to PostgreSQL
# This script migrates data from SQLite to PostgreSQL
# 
# Usage: ./migrate-sqlite-to-postgres.sh <sqlite_db_path> <postgres_host> <postgres_user> <postgres_password> <postgres_db>
# Example: ./migrate-sqlite-to-postgres.sh ./plants.db localhost planty planty_password plantydb

set -e

if [ $# -lt 5 ]; then
  echo "Usage: $0 <sqlite_db_path> <postgres_host> <postgres_user> <postgres_password> <postgres_db> [postgres_port]"
  echo ""
  echo "Example: $0 ./plants.db localhost planty planty_password plantydb"
  echo ""
  echo "This script migrates data from SQLite to PostgreSQL for the Planty application."
  echo ""
  echo "Requirements:"
  echo "  - sqlite3 command-line tool"
  echo "  - psql command-line tool"
  echo "  - Write access to the SQLite database"
  echo "  - Access to the PostgreSQL database"
  exit 1
fi

SQLITE_PATH="$1"
PG_HOST="$2"
PG_USER="$3"
PG_PASSWORD="$4"
PG_DB="$5"
PG_PORT="${6:-5432}"

echo "=========================================="
echo "SQLite to PostgreSQL Migration Script"
echo "=========================================="
echo ""
echo "SQLite database: $SQLITE_PATH"
echo "PostgreSQL: $PG_HOST:$PG_PORT/$PG_DB"
echo ""

# Verify SQLite database exists
if [ ! -f "$SQLITE_PATH" ]; then
  echo "ERROR: SQLite database not found at $SQLITE_PATH"
  exit 1
fi

echo "Step 1: Verifying SQLite database..."
sqlite3 "$SQLITE_PATH" "SELECT name FROM sqlite_master WHERE type='table';" > /dev/null
if [ $? -eq 0 ]; then
  echo "✓ SQLite database is valid"
else
  echo "ERROR: Failed to read SQLite database"
  exit 1
fi

echo ""
echo "Step 2: Exporting schema and data from SQLite..."

# Create a temporary SQL file
TEMP_SQL=$(mktemp)
trap "rm -f $TEMP_SQL" EXIT

# Export SQLite data as SQL
sqlite3 "$SQLITE_PATH" .dump > "$TEMP_SQL"

# Check if schema was exported
if [ ! -s "$TEMP_SQL" ]; then
  echo "ERROR: Failed to export SQLite data"
  exit 1
fi
echo "✓ Exported SQLite data"

echo ""
echo "Step 3: Converting SQLite SQL to PostgreSQL format..."

# Create a converted SQL file
CONVERTED_SQL=$(mktemp)
trap "rm -f $TEMP_SQL $CONVERTED_SQL" EXIT

# Convert SQLite SQL to PostgreSQL format
cat "$TEMP_SQL" | \
  # Remove SQLite-specific pragmas and transaction statements
  sed '/^PRAGMA/d' | \
  sed '/^BEGIN TRANSACTION;/d' | \
  sed '/^COMMIT;/d' | \
  sed 's/AUTOINCREMENT/AUTO_INCREMENT/g' | \
  # Convert SQLite type names to PostgreSQL equivalents
  sed 's/\bINTEGER\b/INTEGER/g' | \
  sed 's/\bREAL\b/NUMERIC/g' | \
  sed 's/\bTEXT\b/TEXT/g' | \
  sed 's/\bBLOB\b/BYTEA/g' | \
  # Remove SQLite's automatic rowid column from inserts
  sed 's/INSERT INTO sqlite_sequence.*;//g' > "$CONVERTED_SQL"

if [ ! -s "$CONVERTED_SQL" ]; then
  echo "ERROR: Failed to convert SQL"
  exit 1
fi
echo "✓ Converted SQL format"

echo ""
echo "Step 4: Connecting to PostgreSQL and importing data..."
echo "  Host: $PG_HOST"
echo "  Port: $PG_PORT"
echo "  User: $PG_USER"
echo "  Database: $PG_DB"
echo ""

# Export PostgreSQL password for psql
export PGPASSWORD="$PG_PASSWORD"

# Check PostgreSQL connection
if ! psql -h "$PG_HOST" -p "$PG_PORT" -U "$PG_USER" -d "$PG_DB" -c "SELECT 1" > /dev/null 2>&1; then
  echo "ERROR: Failed to connect to PostgreSQL"
  echo "Please verify your connection parameters"
  unset PGPASSWORD
  exit 1
fi
echo "✓ Connected to PostgreSQL"

echo ""
echo "Step 5: Importing data into PostgreSQL..."
echo "  WARNING: This will replace existing data in the database!"
read -p "Are you sure? (type 'yes' to continue): " -r
echo ""
if [[ ! $REPLY =~ ^[Yy][Ee][Ss]$ ]]; then
  echo "Migration cancelled."
  unset PGPASSWORD
  exit 1
fi

# Import the converted SQL
if psql -h "$PG_HOST" -p "$PG_PORT" -U "$PG_USER" -d "$PG_DB" -f "$CONVERTED_SQL" > /dev/null 2>&1; then
  echo "✓ Data imported successfully"
else
  echo "ERROR: Failed to import data into PostgreSQL"
  unset PGPASSWORD
  exit 1
fi

echo ""
echo "Step 6: Verifying migration..."

# Count tables in both databases
SQLITE_TABLES=$(sqlite3 "$SQLITE_PATH" "SELECT COUNT(name) FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';" | tr -d '\n')
PG_TABLES=$(psql -h "$PG_HOST" -p "$PG_PORT" -U "$PG_USER" -d "$PG_DB" -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='public';" | tr -d ' \n')

echo "Tables in SQLite: $SQLITE_TABLES"
echo "Tables in PostgreSQL: $PG_TABLES"

if [ "$SQLITE_TABLES" -eq "$PG_TABLES" ]; then
  echo "✓ Table count matches"
else
  echo "WARNING: Table count mismatch (may indicate incomplete migration)"
fi

unset PGPASSWORD

echo ""
echo "=========================================="
echo "Migration completed successfully!"
echo "=========================================="
echo ""
echo "Next steps:"
echo "1. Verify all data is present in PostgreSQL"
echo "2. Test the application with the new database"
echo "3. Keep your SQLite database as a backup"
echo ""
echo "To use the PostgreSQL database with Docker Compose:"
echo "1. Set DB_PASSWORD in your .env file"
echo "2. Run: docker-compose up"
echo ""
