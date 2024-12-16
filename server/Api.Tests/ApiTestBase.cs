using Api.Tests;
using infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PgCtx;
using service;
using Startup;
using Xunit.Abstractions;

public class ApiTestBase : WebApplicationFactory<Program>
{   
    private readonly ITestOutputHelper _outputHelper;
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
        
        ApplicationServices = Services.CreateScope().ServiceProvider; //Triggers ConfigureWebHost
        ApplicationServices.GetRequiredService<Seeder>().Seed().Wait();
        SetupHttpClients();   
    }


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MyDbContext>));
            services.Remove(dbContextDescriptor);

            var proxyDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IProxyConfig));
            services.Remove(proxyDescriptor);

            services.AddDbContext<MyDbContext>(opt =>
            {
                opt.UseNpgsql(PgCtxSetup._postgres.GetConnectionString());
                opt.EnableSensitiveDataLogging(false);
                opt.LogTo(_ => { });
            });
            services.AddSingleton<IProxyConfig, MockProxyConfig>();
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddXUnit(_outputHelper);
        });    }

    private void SetupHttpClients()
    {
        UserHttpClient = CreateClient();
        AdminHttpClient = CreateClient();

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