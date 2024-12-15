using infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class Seeder(MyDbContext context)
{
    public async Task Seed()
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        File.WriteAllText("current_schema.sql", context.Database.GenerateCreateScript());
    }
}