using infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class Seeder(MyDbContext context)
{
    public async Task Seed(bool delete=false)
    {
        if(delete) context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        File.WriteAllText("current_schema.sql", context.Database.GenerateCreateScript());
    }
}