using Infrastructure.Postgres;

namespace Startup.Tests;

public class TestEnvironmentSeeder : ISeeder
{
    public Task Seed()
    {
        return Task.CompletedTask;
    }
}