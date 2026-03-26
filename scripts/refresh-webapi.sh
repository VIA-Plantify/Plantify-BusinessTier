#!/usr/bin/env bash
set -Eeuo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
COMPOSE_FILE="${PROJECT_ROOT}/docker-compose-business.yml"
PROJECT_NAME="plantify-businesstier"
SERVICE="webapi"

cd "${PROJECT_ROOT}"

echo "Stopping ${SERVICE}..."
docker compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" stop "${SERVICE}" || true

echo "Removing old ${SERVICE} container..."
docker compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" rm -f "${SERVICE}" || true

echo "Rebuilding and starting fresh ${SERVICE}..."
docker compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" up --build --force-recreate -d --no-deps "${SERVICE}"

echo
docker compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" ps "${SERVICE}"
echo
docker compose -p "${PROJECT_NAME}" -f "${COMPOSE_FILE}" logs --tail=50 "${SERVICE}"