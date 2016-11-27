#!/bin/bash
set -e

cd /app/src/SimplCommerce.WebHost && dotnet ef database update

psql --username postgres -d simplcommerce -a -f /app/src/Database/StaticData_Postgres.sql

cd /app/src/SimplCommerce.WebHost && dotnet run