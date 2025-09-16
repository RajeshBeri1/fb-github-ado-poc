#!/bin/bash
start=$PWD

echo "Resetting solution to a clean state"
./reset-npm.sh

# removing existing services on docker
echo -n "Removing current storage services from docker..."
cd $start
docker-compose down --volumes
echo "done"

# TODO: Delete database files on docker volume?
# \\wsl.localhost\docker-desktop-data\data\docker\volumes\flowchart_tools_daily_mssql\_data\data

# recreating services on docker
echo -n "Creating storage services on docker..."
cd $start
./migrate.sh
echo "done"