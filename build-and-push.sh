#!/bin/bash
set -e

echo "🚀 Building and pushing ChanBoard images to Docker Hub"

# Login to Docker Hub
echo "🔑 Logging into Docker Hub..."
docker login

# Build and push dev images
echo ""
echo "📦 Building DEV images..."
docker-compose -f docker-compose.dev.yml build

echo "⬆️  Pushing DEV images to Docker Hub..."
docker-compose -f docker-compose.dev.yml push

# Build and push prod images
echo ""
echo "📦 Building PROD images..."
docker-compose -f docker-compose.prod.yml build

echo "⬆️  Pushing PROD images to Docker Hub..."
docker-compose -f docker-compose.prod.yml push

echo ""
echo "✅ Done! All images pushed to Docker Hub"
echo ""
echo "Images pushed:"
echo "  - anthonyr001/chanboard-api:dev"
echo "  - anthonyr001/chanboard-nginx:dev"
echo "  - anthonyr001/chanboard-api:prod"
echo "  - anthonyr001/chanboard-nginx:prod"

###How to run this script:
# chmod +x build-and-push.sh
# ./build-and-push.sh