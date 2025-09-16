#!/bin/bash

echo 'Start docker services'
docker-compose up -d mssql redis azurite

sleep 5

cd WebAPI/
echo 'Run database migration'
dotnet-ef database update
