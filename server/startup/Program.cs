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

        var proxyConfiguration = new TcpProxyConfiguration
        {
            PublicHost = new Host
            {
                IpAddress = IPAddress.Parse("0.0.0.0"),
                Port = 8080
            },
            HttpHost = new Host
            {
                IpAddress = IPAddress.Loopback,
                Port = 5000
            },
            WebSocketHost = new Host
            {
                IpAddress = IPAddress.Loopback,
                Port = 8181
            }
        };
        new TcpProxyServer(proxyConfiguration).Start();


        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();

        app.Run();
    }
}