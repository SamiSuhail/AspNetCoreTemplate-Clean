#!/bin/bash
set -e

# Function to wait for containers to exit and check status
wait_for_exit() {
    local compose_file=$1
    echo "Waiting for containers from $compose_file to exit..."
    
    # Wait for all containers to stop
    while true; do
        local running=$(docker-compose -f "$compose_file" --env-file build/docker-compose.env ps --services --filter "status=running")
        if [ -z "$running" ]; then
            break
        fi
        sleep 5
    done

    # Check exit codes
    local failed=false
    while IFS= read -r container; do
        if [ ! -z "$container" ]; then
            local exit_code=$(docker inspect "$container" --format='{{.State.ExitCode}}')
            if [ "$exit_code" != "0" ]; then
                echo "Container $container failed with exit code $exit_code"
                failed=true
            fi
        fi
    done < <(docker-compose -f "$compose_file" --env-file build/docker-compose.env ps -q)

    if [ "$failed" = true ]; then
        exit 1
    fi
}

wait_for_running_or_exit() {
    local compose_file=$1
    echo "Waiting for containers from $compose_file to be running or exited..."
    
    # Wait for containers to be either running or exited
    while true; do
        local starting=$(docker-compose -f "$compose_file" --env-file build/docker-compose.env ps --services --filter "status=starting")
        if [ -z "$starting" ]; then
            break
        fi
        sleep 5
    done

    # Check for any failed containers
    local failed=false
    while IFS= read -r container; do
        if [ ! -z "$container" ]; then
            local status=$(docker inspect "$container" --format='{{.State.Status}}')
            local exit_code=$(docker inspect "$container" --format='{{.State.ExitCode}}')
            
            if [ "$status" = "exited" ] && [ "$exit_code" != "0" ]; then
                echo "Container $container failed with exit code $exit_code"
                failed=true
            fi
        fi
    done < <(docker-compose -f "$compose_file" --env-file build/docker-compose.env ps -q)

    if [ "$failed" = true ]; then
        exit 1
    fi
}

echo "Step 1: Running build containers..."
docker-compose -f build/docker-compose.build.yml --env-file build/docker-compose.env up --build -d
wait_for_exit build/docker-compose.build.yml

echo "Step 2: Running root containers..."
docker-compose -f build/docker-compose.root.yml --env-file build/docker-compose.env up --build -d
wait_for_running_or_exit build/docker-compose.root.yml

echo "Step 3: Running app containers..."
docker-compose -f build/docker-compose.app.yml --env-file build/docker-compose.env up --build -d
wait_for_running_or_exit build/docker-compose.app.yml

echo "Step 4: Running system tests..."
docker-compose -f build/docker-compose.testssystem.yml --env-file build/docker-compose.env up --build -d

echo "Done!" 