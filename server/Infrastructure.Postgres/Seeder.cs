using infrastructure;

namespace Infrastructure.Repositories;

public class Seeder(MyDbContext context)
{
    public async Task Seed()
    {
        context.Database.EnsureCreated();
    }
}