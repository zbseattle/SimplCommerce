#!/bin/bash
set -e

cd src/SimplCommerce.WebHost && dotnet ef database update

cd src/SimplCommerce.WebHost && dotnet run