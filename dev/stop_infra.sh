#!/bin/bash

cd "$(dirname "$0")"

project_name=real_estate_booking_infra

echo "Stop ${project_name} infrastructure..."
docker compose -f docker-compose.infra.yml --env-file .env_infra -p "${project_name}" down

