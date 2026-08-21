#!/usr/bin/env bash
set -euo pipefail

# Waits for SQL Server to accept connections, then runs init-databases.sql
# inside the sqlserver container to create all dev/test databases.

SA_PASSWORD="${SA_PASSWORD:-Test12345}"
CONTAINER_NAME="${CONTAINER_NAME:-mssbase-sqlserver}"

echo "Waiting for SQL Server to become available..."
for i in $(seq 1 60); do
  if docker exec "$CONTAINER_NAME" /opt/mssql-tools18/bin/sqlcmd -C -S 127.0.0.1 -U sa -P "$SA_PASSWORD" -Q "SELECT 1" >/dev/null 2>&1; then
    echo "SQL Server is up."
    break
  fi
  sleep 2
  if [ "$i" -eq 60 ]; then
    echo "Timed out waiting for SQL Server." >&2
    exit 1
  fi
done

echo "Creating databases..."
docker exec -i "$CONTAINER_NAME" /opt/mssql-tools18/bin/sqlcmd -C -S 127.0.0.1 -U sa -P "$SA_PASSWORD" < "$(dirname "$0")/init-databases.sql"

echo "Databases created."
