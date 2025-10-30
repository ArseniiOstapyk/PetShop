#!/bin/sh
set -e

echo "Waiting for database to be ready at $DB_HOST:$DB_PORT..."

until nc -z "$DB_HOST" "$DB_PORT"; do
  sleep 1
done

echo "Database is up — starting application."
exec "$@"