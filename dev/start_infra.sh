#!/bin/bash

cd "$(dirname "$0")"

echo "Starting local development services..."
docker compose -f docker-compose.infra.yml --env-file .env_infra -p real_estate_booking_infra up -d
