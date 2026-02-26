#!/bin/bash
set -e

echo "🚀 Building and pushing ChanBoard API images"

# Login to Docker Hub
docker login

# Build and push server image
echo "📦 Building server image..."
docker-compose --profile server build
docker-compose --profile server push

# Build and push Raspberry Pi image
echo "📦 Building Raspberry Pi image..."
docker-compose --profile pi build
docker-compose --profile pi push

echo "✅ Done! Images pushed to Docker Hub"
echo ""
echo "Server deployment:"
echo "  docker-compose --profile server up -d"
echo ""
echo "Raspberry Pi deployment:"
echo "  docker-compose --profile pi up -d"

###How to run this script in the terminal:
#chmod +x deploy-compose.sh
#./deploy-compose.sh