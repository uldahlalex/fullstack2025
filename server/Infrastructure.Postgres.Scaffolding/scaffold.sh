#!/bin/bash
dotnet ef dbcontext scaffold \
  "Server=localhost;Database=testdb;User Id=testuser;Password=testpass;" \
  Npgsql.EntityFrameworkCore.PostgreSQL \
  --output-dir ../Application/Models  \
  --context-dir . \
  --context MyDbContext  \
  --no-onconfiguring \
  --force