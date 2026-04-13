#!/usr/bin/env bash
set -Eeuo pipefail

NETWORK="plantify-network"

echo "Checking Docker..."
docker info >/dev/null 2>&1

if ! docker network inspect "$NETWORK" >/dev/null 2>&1; then
  echo "Creating Docker network: $NETWORK"
  docker network create --driver bridge "$NETWORK"
else
  echo "Docker network already exists: $NETWORK"
fi

echo "Base Docker setup complete."