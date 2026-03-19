#!/bin/bash
set -e

echo "🚀 Deploying ChanBoard PROD environment"

# Login to Docker Hub (if needed)
echo "🔑 Logging into Docker Hub..."
docker login

# Pull latest prod images
echo "⬇️  Pulling latest PROD images from Docker Hub..."
docker-compose -f docker-compose.prod.yml pull

# Stop existing containers
echo "🛑 Stopping existing PROD containers..."
docker-compose -f docker-compose.prod.yml down

# Start services
echo "▶️  Starting PROD services..."
docker-compose -f docker-compose.prod.yml up -d

echo ""
echo "✅ PROD environment deployed successfully!"
echo ""
echo "Services running:"
echo "  - API: http://localhost:8080"
echo "  - Nginx: http://localhost:80, https://localhost:443"
echo "  - Postgres: localhost:5432"
echo "  - Blob Storage: localhost:10000"
echo ""
echo "Check logs: docker-compose -f docker-compose.prod.yml logs -f"

###How to run this script:
# chmod +x pull-and-deploy-prod.sh
# ./pull-and-deploy-prod.sh