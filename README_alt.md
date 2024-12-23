## Running

#### A: Via DevContainer

(Requires Buildx + Docker installed)

Mount via devcontainer.json:

### B: Via docker compose

### C: Run locally

## Deploying

### A: 

### Common actions:

#### Scaffolding:

[//]: # (The code block below should insert the raw file contents from ./server/Infrastructure.Postgres.Scaffolding/scaffold.sh )
```

```
[//]: # (The code block below should insert the raw file contents from ./server/Infrastructure.Postgres.Scaffolding/scaffold.sh )
```
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
  ```
## Running

#### A: Via DevContainer

(Requires Buildx + Docker installed)

Mount via devcontainer.json:

### B: Via docker compose

### C: Run locally

## Deploying

### A: 

### Common actions:

#### Scaffolding:

[//]: # (The code block below should insert the raw file contents from ./server/Infrastructure.Postgres.Scaffolding/scaffold.sh )
```

```
