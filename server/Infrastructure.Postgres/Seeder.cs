using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres;

public class Seeder(MyDbContext context)
{
    public async Task Seed(bool delete = false)
    {
        if (delete) context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        File.WriteAllText("current_schema.sql", context.Database.GenerateCreateScript());
    }
}