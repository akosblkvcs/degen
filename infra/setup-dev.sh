#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "=== Degen Development Setup ==="
echo ""

echo "[1/2] Starting PostgreSQL + RabbitMQ..."
docker compose -f "$SCRIPT_DIR/docker/docker-compose.infra.yml" up -d

echo "[2/2] Waiting for services to be healthy..."
until docker exec degen-postgres pg_isready -U degen -q 2>/dev/null; do
    sleep 1
done
echo "  PostgreSQL is ready."

until docker exec degen-rabbitmq rabbitmq-diagnostics check_running -q 2>/dev/null; do
    sleep 1
done
echo "  RabbitMQ is ready."

echo ""
echo "=== Infrastructure ready ==="
echo ""
echo "  PostgreSQL:    localhost:5432  (degen/devpassword)"
echo "  RabbitMQ:      localhost:5672  (degen/devpassword)"
echo "  RabbitMQ UI:   http://localhost:15672"
echo ""
echo "Run api-core (migrations apply automatically):"
echo "  cd apps/api-core && dotnet run --project src/Degen.Api"
