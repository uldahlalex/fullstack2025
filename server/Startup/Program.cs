using System.Net;
using Infrastructure.Repositories;
using realtimeapi;
using restapi;
using service;
using WebSocketProxy;
using Host = WebSocketProxy.Host;


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