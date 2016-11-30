#!/bin/bash
set -e

cd /app/src/SimplCommerce.WebHost && dotnet ef database update

echo 'select count(*) from myvalue;' | psql --username postgres -d simplcommerce -t > varfile.txt
if [ `cat varfile.txt` eq 0 ]; then
  echo "first time --> create init data"
  psql --username postgres -d simplcommerce -a -f /app/src/Database/StaticData_Postgres.sql
else
  echo "already have init data"
fi

cd /app/src/SimplCommerce.WebHost && dotnet run
