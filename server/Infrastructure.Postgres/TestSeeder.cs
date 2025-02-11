namespace Infrastructure.Postgres;

public class TestSeeder : ISeeder
{
    public Task Seed()
    {
        return Task.CompletedTask;
    }
}