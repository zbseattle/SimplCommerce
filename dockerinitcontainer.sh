#!/bin/bash
set -e

cd /app/src/SimplCommerce.WebHost && dotnet ef database update

echo 'select count(*) from "Core_User";' | psql --username postgres -d simplcommerce -t > /tmp/varfile.txt
if [ `cat /tmp/varfile.txt` -eq 0 ]; then
  echo "first time --> create init data"
  psql --username postgres -d simplcommerce -a -f /app/src/Database/StaticData_Postgres.sql
else
  echo "already have init data"
fi

cd /app/src/SimplCommerce.WebHost && dotnet run
