#!/bin/bash
dotnet ef dbcontext scaffold \
  "Server=localhost;Database=testdb;User Id=testuser;Password=testpass;" \
  Npgsql.EntityFrameworkCore.PostgreSQL \
  --output-dir ../Application/Models/Entities  \
  --context-dir . \
  --context MyDbContext  \
  --no-onconfiguring \
  --namespace Application.Models.Entities \
  --context-namespace  Infrastructure.Postgres.Scaffolding \
  --force