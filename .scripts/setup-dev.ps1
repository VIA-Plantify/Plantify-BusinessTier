$NETWORK = "backend"

docker info *> $null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker is not running or not installed."
    exit 1
}

docker network inspect $NETWORK *> $null

if ($LASTEXITCODE -ne 0) {
    Write-Host "Creating Docker network: $NETWORK"
    docker network create $NETWORK
} else {
    Write-Host "Docker network already exists: $NETWORK"
}

Write-Host "Base Docker setup complete."