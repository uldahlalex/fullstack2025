using System.Net.Http.Headers;
using infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PgCtx;
using service;
using Startup;
using Xunit.Abstractions;

namespace Api.Tests;

public class ApiTestBase : WebApplicationFactory<Program>
{   private readonly ITestOutputHelper _outputHelper;
    public PgCtxSetup<MyDbContext> PgCtxSetup;
    public HttpClient UserHttpClient { get; private set; }
    public HttpClient AdminHttpClient { get; private set; }
    public string UserJwt { get; private set; }
    public string AdminJwt { get; private set; }
    public IServiceProvider ApplicationServices { get; private set; }

    public ApiTestBase(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        PgCtxSetup = new PgCtxSetup<MyDbContext>();
        
        // Create clients after base configuration is done
        InitializeClients();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            // First remove any services you want to replace
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MyDbContext>));
            services.Remove(dbContextDescriptor);

            var proxyDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IProxyConfig));
            services.Remove(proxyDescriptor);

            // Add test-specific services
            services.AddDbContext<MyDbContext>(opt =>
            {
                opt.UseNpgsql(PgCtxSetup._postgres.GetConnectionString());
                opt.EnableSensitiveDataLogging(false);
                opt.LogTo(_ => { });
            });
            services.AddSingleton<IProxyConfig, MockProxyConfig>();

            // Configure test-specific app options
            services.Configure<AppOptions>(options => 
            {
                options.Seed = true;  // or whatever you need for testing
                // Set other options as needed
            });
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddXUnit(_outputHelper);
        });
    }

    private void InitializeClients()
    {
        // Create and configure HTTP clients
        UserHttpClient = CreateClient();
        AdminHttpClient = CreateClient();

        // Get services
        ApplicationServices = Services.CreateScope().ServiceProvider;

        // Setup authentication if needed
        // var tokenService = ApplicationServices.GetRequiredService<ITokenClaimsService>();
        // UserJwt = await tokenService.GetTokenAsync("testuser");
        // AdminJwt = await tokenService.GetTokenAsync("adminuser");

        // Configure client headers
        // UserHttpClient.DefaultRequestHeaders.Authorization = 
        //     new AuthenticationHeaderValue("Bearer", UserJwt);
        // AdminHttpClient.DefaultRequestHeaders.Authorization = 
        //     new AuthenticationHeaderValue("Bearer", AdminJwt);
    }

}