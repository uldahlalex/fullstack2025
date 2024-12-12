using System.Text.Json;
using Api.Realtime;
using Api.Rest;
using Api.Rest.Controllers;
using infrastructure;
using Infrastructure.Mqtt;
using Infrastructure.Repositories;
using Infrastructure.Websocket;
using Microsoft.AspNetCore.WebSockets;
using service;
using service.Extensions;
using Startup.Extensions;

namespace Startup;

public static class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        var options = builder.AddAppOptions();
        builder.Services.AddDataSourceAndRepositories();
        builder.Services.AddWebsocketInfrastructure();
        builder.Services.AddMqttInfrastructure();
        
        builder.Services.AddApplicationServices();
        
        builder.AddDependenciesForRestApi();
        builder.AddDependenciesForRealtimeApi();
        
        builder.WebHost.UseUrls("http://*:5000");

        var app = builder.Build();

        new ProxyConfig().StartProxyServer();


        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();

        
        if (options.Seed)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
            seeder.Seed();
        }
        app.Run();
    }
}