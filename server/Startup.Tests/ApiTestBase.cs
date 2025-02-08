using Infrastructure.Postgres;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PgCtx;
using Startup;
using Xunit.Abstractions;

namespace Startup.Tests;

public class ApiTestBase(ITestOutputHelper outputHelper) : WebApplicationFactory<Program>
{
    private readonly PgCtxSetup<MyDbContext> _pgCtxSetup = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(ConfigureTestServices);
        builder.ConfigureLogging(ConfigureTestLogging);
    }

    private void ConfigureTestServices(WebHostBuilderContext context, IServiceCollection services)
    {
        RemoveExistingService<DbContextOptions<MyDbContext>>(services);
        RemoveExistingService<IProxyConfig>(services);
        RemoveExistingService<ISeeder>(services);

        services.AddDbContext<MyDbContext>(opt =>
        {
            opt.UseNpgsql(_pgCtxSetup._postgres.GetConnectionString());
            opt.EnableSensitiveDataLogging();
            opt.LogTo(_ => { });
        });
        services.AddSingleton<IProxyConfig, MockProxyConfig>();
        services.AddSingleton<ISeeder, TestSeeder>();
    }

    private void RemoveExistingService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }

    private void ConfigureTestLogging(ILoggingBuilder logging)
    {
        logging.ClearProviders();
        logging.AddXUnit(outputHelper);
    }
}