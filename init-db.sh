#!/bin/bash
set -e

echo "Waiting for PostgreSQL to start..."
until pg_isready -U postgres > /dev/null 2>&1; do
  sleep 2
done

if [ -f "/docker-entrypoint-initdb.d/dbbackup.sql" ]; then
  echo "Restoring SQL backup..."
  psql -U postgres -d PetShop -f /docker-entrypoint-initdb.d/dbbackup.sql
else
  echo "No SQL dump found, skipping restore."
fi

echo "Database initialization complete."