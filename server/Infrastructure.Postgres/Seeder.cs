using infrastructure;
using Microsoft.EntityFrameworkCore;
using PgCtx;

namespace Infrastructure.Repositories;

public class Seeder(MyDbContext context)
{
    public void Seed()
    {


        context.Database.EnsureCreated();
    }
}