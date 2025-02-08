using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Startup.Tests.Testing.Tests;

public class Connectivity(ITestOutputHelper testOutputHelper) : ApiTestBase(testOutputHelper)
{
    [Fact]
    public async Task Api_And_Tests_Are_Using_Same_DB()
    {
        using var scope = Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        // Log the test context's connection string
        var testConnectionString = ctx.Database.GetConnectionString();
        testOutputHelper.WriteLine($"Test connection string: {testConnectionString}");

        // Log the API's connection string
        var client = CreateClient();
        using var apiScope = Services.CreateScope();
        var apiContext = apiScope.ServiceProvider.GetRequiredService<MyDbContext>();
        var apiConnectionString = apiContext.Database.GetConnectionString();
        testOutputHelper.WriteLine($"API connection string: {apiConnectionString}");

        // Rest of your test code...
    }
}