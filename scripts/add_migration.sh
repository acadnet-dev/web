#!/bin/bash

if [ $# -lt 1 ]; then
    echo "Usage: $0 <migration_name>"
    exit 1
fi

dotnet ef migrations add $1 --project Acadnet.Data --startup-project Acadnet.Web