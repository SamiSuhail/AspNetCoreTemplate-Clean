#!/bin/bash
set -e

BUILD_DIR="build"
ENV_FILE="$BUILD_DIR/docker-compose.env"

# Function to wait for containers and check status
wait_for_containers() {
    local compose_file=$1
    local acceptable_statuses=$2 # Pipe-separated list of acceptable statuses
    
    # Validate input
    if [ -z "$acceptable_statuses" ]; then
        echo "Error: acceptable_statuses must be provided as pipe-separated list (e.g. 'exited|dead')"
        exit 1
    fi

    echo "Waiting for containers from $compose_file to reach acceptable status..."
    local timeout=30 # 30 seconds timeout
    local start_time=$SECONDS
    
    while true; do
        # Get containers not in acceptable status by excluding those matching acceptable statuses
        local pending=""
        for status in $(echo $acceptable_statuses | tr "|" "\n"); do
            if [ -z "$pending" ]; then
                pending=$(docker-compose -f "$compose_file" --env-file "$ENV_FILE" ps --services --filter "status=$status")
            else
                pending=$(echo "$pending" | grep -v -f <(docker-compose -f "$compose_file" --env-file "$ENV_FILE" ps --services --filter "status=$status"))
            fi
        done
        
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
            
            if [ "$exit_code" != "0" ]; then
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

echo "Step 2: Running root containers..."
wait_for_containers "$BUILD_DIR/docker-compose.build.yml" "exited"
docker-compose -f "$BUILD_DIR/docker-compose.root.yml" --env-file "$ENV_FILE" up --build -d

echo "Step 3: Running app containers..."
wait_for_containers "$BUILD_DIR/docker-compose.root.yml" "up"
docker-compose -f "$BUILD_DIR/docker-compose.app.yml" --env-file "$ENV_FILE" up --build -d

echo "Step 4: Running system tests..."
wait_for_containers "$BUILD_DIR/docker-compose.app.yml" "up|exited"
docker-compose -f "$BUILD_DIR/docker-compose.testssystem.yml" --env-file "$ENV_FILE" up --build -d

echo "Done!"