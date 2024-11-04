#!/bin/bash
set -e

BUILD_DIR="build"
ENV_FILE="$BUILD_DIR/docker-compose.env"

# Function to wait for containers and check status
wait_for_containers() {
    local compose_file=$1
    local wait_type=$2
    if [ "$wait_type" != "completed" ] 
    && [ "$wait_type" != "running" ]; then
        echo "Error: wait_type must be either 'completed' or 'running'"
        exit 1
    fi

    echo "Waiting for containers from $compose_file to be ${wait_type}..."
    local timeout=30 # 30 seconds timeout
    local start_time=$SECONDS
    
    while true; do
        if [ "$wait_type" = "completed" ]; then
            local pending=$(docker-compose -f "$compose_file" --env-file "$ENV_FILE" ps --services --filter "status=running")
        else
            local pending=$(docker-compose -f "$compose_file" --env-file "$ENV_FILE" ps --services --filter "status=starting")
        fi
        
        if [ -z "$pending" ]; then
            break
        fi
        
        # Check if we've exceeded timeout
        local elapsed=$(( SECONDS - start_time ))
        if [ $elapsed -gt $timeout ]; then
            echo "Timeout waiting for containers. Current state:"
            docker-compose -f "$compose_file" --env-file "$ENV_FILE" ps
            exit 1
        fi
        
        sleep 5
    done

    # Check for any failed containers
    local failed=false
    while IFS= read -r container; do
        if [ ! -z "$container" ]; then
            local status=$(docker inspect "$container" --format='{{.State.Status}}')
            local exit_code=$(docker inspect "$container" --format='{{.State.ExitCode}}')

            echo "Container $container failed with exit code $exit_code"
            failed=true
            fi
        fi
    done < <(docker-compose -f "$compose_file" --env-file "$ENV_FILE" ps -q)

    if [ "$failed" = true ]; then
        exit 1
    fi
}

echo "Step 1: Running build containers..."
docker-compose -f "$BUILD_DIR/docker-compose.build.yml" --env-file "$ENV_FILE" up --build -d
wait_for_containers "$BUILD_DIR/docker-compose.build.yml" "completed"

echo "Step 2: Running root containers..."
docker-compose -f "$BUILD_DIR/docker-compose.root.yml" --env-file "$ENV_FILE" up --build -d
wait_for_containers "$BUILD_DIR/docker-compose.root.yml" "running"

echo "Step 3: Running app containers..."
docker-compose -f "$BUILD_DIR/docker-compose.app.yml" --env-file "$ENV_FILE" up --build -d
wait_for_containers "$BUILD_DIR/docker-compose.app.yml" "running"

echo "Step 4: Running system tests..."
docker-compose -f "$BUILD_DIR/docker-compose.testssystem.yml" --env-file "$ENV_FILE" up --build -d

echo "Done!"