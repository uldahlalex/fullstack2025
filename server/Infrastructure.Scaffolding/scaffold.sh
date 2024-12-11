#!/bin/bash
dotnet ef dbcontext scaffold \
  "Server=localhost;Database=testdb;User Id=testuser;Password=testpass;" \
  Npgsql.EntityFrameworkCore.PostgreSQL \
  --output-dir ../core/Models  \
  --context-dir . \
  --context MyDbContext  \
  --no-onconfiguring \
  --force