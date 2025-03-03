using Application.Models;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Postgres;

public class Seeder(MyDbContext context, IOptionsMonitor<AppOptions> optionsMonitor) : ISeeder
{
    public async Task Seed()
    {
        await context.Database.EnsureCreatedAsync();
        await File.WriteAllTextAsync("../Infrastructure.Postgres.Scaffolding/current_schema.sql", context.Database.GenerateCreateScript());
    }
}