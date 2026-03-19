#!/bin/bash
set -e

echo "🚀 Deploying ChanBoard DEV environment"

# Login to Docker Hub (if needed)
echo "🔑 Logging into Docker Hub..."
docker login

# Pull latest dev images
echo "⬇️  Pulling latest DEV images from Docker Hub..."
docker-compose -f docker-compose.dev.yml pull

# Stop existing containers
echo "🛑 Stopping existing DEV containers..."
docker-compose -f docker-compose.dev.yml down

# Start services
echo "▶️  Starting DEV services..."
docker-compose -f docker-compose.dev.yml up -d

echo ""
echo "✅ DEV environment deployed successfully!"
echo ""
echo "Services running:"
echo "  - API: http://localhost:5008"
echo "  - Nginx: http://localhost:5080"
echo "  - Postgres: localhost:5433"
echo "  - Blob Storage: localhost:10001"
echo ""
echo "Check logs: docker-compose -f docker-compose.dev.yml logs -f"

###How to run this script:
# chmod +x pull-and-deploy-dev.sh
# ./pull-and-deploy-dev.sh