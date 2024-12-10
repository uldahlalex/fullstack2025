using Infrastructure.Repositories;
using realtimeapi;
using restapi;
using service;

public static class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddDataSource("Host=localhost;Database=testdb;Username=testuser;Password=testpass");
        builder.Services.AddRepositories();
        builder.Services.AddApplicationServices();
        builder.AddDependenciesForRestApi();
        builder.AddDependenciesForRealtimeApi();
        
        
        var app = builder.Build();

        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();

        app.Run();
    }
}