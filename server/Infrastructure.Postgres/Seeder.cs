using Application.Models;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Postgres;

public class Seeder(MyDbContext context, IOptionsMonitor<AppOptions> optionsMonitor)
{
    public async Task Seed()
    {
        if (optionsMonitor.CurrentValue.Seed)
        {
            context.Database.ExecuteSql($"DROP SCHEMA if exists jerneif CASCADE; CREATE SCHEMA jerneif;");
        }

        context.Database.EnsureCreated();
        File.WriteAllText("current_schema.sql", context.Database.GenerateCreateScript());
    }
}