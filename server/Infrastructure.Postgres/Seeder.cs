using Application.Models;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Postgres;

public class Seeder(MyDbContext context, IOptionsMonitor<AppOptions> optionsMonitor) : ISeeder
{
    public async Task Seed()
    {
        await context.Database.ExecuteSqlRawAsync("DROP SCHEMA IF EXISTS jerneif CASCADE;");
        await context.Database.EnsureCreatedAsync();
        await File.WriteAllTextAsync("current_schema.sql", context.Database.GenerateCreateScript());
    }
}