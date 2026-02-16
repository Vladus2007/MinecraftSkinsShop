#!/usr/bin/env bash
set -euo pipefail

# Wait for Postgres to become available before running the app.
# Usage: the script will exec whatever command is passed after the checks.

host="${DB_HOST:-postgres}"
port="${DB_PORT:-5432}"
retries="${RETRIES:-60}"
count=0

echo "Waiting for ${host}:${port}..."
while ! (echo > /dev/tcp/${host}/${port}) >/dev/null 2>&1; do
  count=$((count+1))
  if [ "$count" -ge "$retries" ]; then
    echo "Timeout waiting for ${host}:${port} after ${retries} seconds"
    exit 1
  fi
  sleep 1
done

echo "${host}:${port} is available - continuing"

exec "$@"
