# Enable BuildKit
export DOCKER_BUILDKIT=1

# Build
docker build --progress=plain -t backend .