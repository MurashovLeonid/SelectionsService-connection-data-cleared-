#!/usr/bin/env bash

cd "../src";
echo 'Adding migration....';
dotnet ef migrations add $1 --project Superbrands.Selection.Infrastructure --startup-project Superbrands.Selection.WebApi