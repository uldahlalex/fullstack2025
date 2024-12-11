using Api.Realtime;
using Api.Rest;
using Infrastructure.Repositories;
using service;

namespace Startup;

public static class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDataSourceAndRepositories(
            "Host=localhost;Database=testdb;Username=testuser;Password=testpass");
        builder.Services.AddApplicationServices();
        builder.AddDependenciesForRestApi();
        builder.AddDependenciesForRealtimeApi();
        builder.WebHost.UseUrls("http://*:5000");

        var app = builder.Build();

        new ProxyConfig().StartProxyServer();


        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();

        app.Run();
    }
}