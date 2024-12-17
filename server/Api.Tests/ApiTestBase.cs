using Infrastructure.Postgres;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PgCtx;
using Startup;
using Xunit.Abstractions;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;

namespace Api.Tests;

public class ApiTestBase : WebApplicationFactory<Program>
{
    private readonly ITestOutputHelper _outputHelper;
    public PgCtxSetup<MyDbContext> PgCtxSetup;
    public IServiceProvider ApplicationServices { get; }
    public HttpClient UserHttpClient { get; private set; }
    public HttpClient AdminHttpClient { get; private set; }
    public string UserJwt { get; private set; }
    public string AdminJwt { get; private set; }

    public ApiTestBase(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        PgCtxSetup = new PgCtxSetup<MyDbContext>();

        // Set environment to Testing
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        ApplicationServices = Services.CreateScope().ServiceProvider;
        ApplicationServices.GetRequiredService<Seeder>().Seed().Wait();
        SetupHttpClients();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Configure test-specific configuration
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // config.SetBasePath(Directory.GetCurrentDirectory())
            //       .AddJsonFile("appsettings.Testing.json", optional: false)
            //       .AddEnvironmentVariables();
        });

        // Configure Services
        builder.ConfigureServices((context, services) =>
        {
            // Call the original Program.ConfigureServices
            Program.ConfigureServices(services, context.Configuration, context.HostingEnvironment);

            // Remove and replace database context
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MyDbContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            // Remove and replace proxy config
            var proxyDescriptor = services.Where(
                d => d.ServiceType == typeof(IProxyConfig)).ToList();
            foreach (var descriptor in proxyDescriptor)
                services.Remove(descriptor);

            // Add test database context
            services.AddDbContext<MyDbContext>(opt =>
            {
                opt.UseNpgsql(PgCtxSetup._postgres.GetConnectionString());
                opt.EnableSensitiveDataLogging(false);
                opt.LogTo(_ => { });
            });

            // Add mock proxy config
            services.AddSingleton<IProxyConfig, MockProxyConfig>();
        });

        // Configure the application
        // builder.Configure(app =>
        // {
        //     Program.ConfigureMiddleware(app.);
        // });

        // Configure logging
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddXUnit(_outputHelper);
            logging.AddFilter("Microsoft.AspNetCore.Routing", LogLevel.Debug);
            logging.AddFilter("Microsoft.AspNetCore.Mvc", LogLevel.Debug);
        });
    }

    private void SetupHttpClients()
    {
        UserHttpClient = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        AdminHttpClient = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Uncomment and modify when implementing authentication
        /*
        // Get token service
        var tokenService = ApplicationServices.GetRequiredService<ITokenClaimsService>();
        
        // Generate tokens
        UserJwt = await tokenService.GetTokenAsync("testuser");
        AdminJwt = await tokenService.GetTokenAsync("adminuser");

        // Set authentication headers
        UserHttpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", UserJwt);
        AdminHttpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", AdminJwt);
        */
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            UserHttpClient?.Dispose();
            AdminHttpClient?.Dispose();
            // PgCtxSetup?.Dispose();
        }

        base.Dispose(disposing);
    }
}