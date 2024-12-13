﻿using System.Net.Http.Headers;
using infrastructure;
using Infrastructure.Repositories;
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
{
    public ApiTestBase(ITestOutputHelper outputHelper)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        _outputHelper = outputHelper;
        PgCtxSetup = new PgCtxSetup<MyDbContext>();
        ApplicationServices = base.Services.CreateScope().ServiceProvider;
        ApplicationServices.GetRequiredService<Seeder>().Seed().GetAwaiter().GetResult(); //todo is already being called? how does webhost actually work
        // var tokenService = ApplicationServices.GetRequiredService<ITokenClaimsService>();
        // var userManager = ApplicationServices.GetRequiredService<UserManager<IdentityUser>>();


        // User = userManager.GetUsersInRoleAsync(Role.Reader).Result.First();
        // Admin = userManager.GetUsersInRoleAsync(Role.Admin).Result.First();
        // UserJwt = tokenService.GetTokenAsync(User.UserName!).Result;
        // AdminJwt = tokenService.GetTokenAsync(Admin.UserName!).Result;
        UserHttpClient = CreateClient();
        UserHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserJwt);
        AdminHttpClient = CreateClient();
        AdminHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AdminJwt);
    }

    public IdentityUser Admin { get; set; }

    public IdentityUser User { get; set; }


    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<MyDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            var proxy = services.SingleOrDefault(d => d.ServiceType == typeof(ProxyConfig));
            if (proxy != null) services.Remove(proxy);


            services.AddDbContext<MyDbContext>(opt =>
            {
                opt.UseNpgsql(PgCtxSetup._postgres.GetConnectionString());
                opt.EnableSensitiveDataLogging(false);
                opt.LogTo(_ => { });
            });
        }).ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddXUnit(_outputHelper);
        });

        return base.CreateHost(builder);
    }

    #region properties

    public PgCtxSetup<MyDbContext> PgCtxSetup;
    private readonly ITestOutputHelper _outputHelper;
    public HttpClient UserHttpClient { get; set; }
    public HttpClient AdminHttpClient { get; set; }

    public string UserJwt { get; set; }
    public string AdminJwt { get; set; }

    public IServiceProvider ApplicationServices { get; set; }

    #endregion
}