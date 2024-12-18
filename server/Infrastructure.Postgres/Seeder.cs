using Application.Models;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Postgres;

public interface ISeeder
{
    Task Seed();
}

public class TestSeeder : ISeeder
{
    public Task Seed()
    {
        return Task.CompletedTask;
    }
}

public class Seeder(MyDbContext context, IOptionsMonitor<AppOptions> optionsMonitor) : ISeeder
{
    public async Task Seed()
    {
        context.Database.ExecuteSqlRaw($"DROP SCHEMA jerneif CASCADE; CREATE SCHEMA jerneif;");
        context.Database.EnsureCreated();
        File.WriteAllText("current_schema.sql", context.Database.GenerateCreateScript());
    }
}