using realtimeapi;
using restapi;
using service;

public static class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddDataSource();
        builder.Services.AddApplicationServices();
        builder.AddDependenciesForRestApi();
        builder.AddDependenciesForRealtimeApi();
        
        
        var app = builder.Build();

        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();
        app.UseCors(opts => opts.AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(_ => true));

        app.Run();
    }
}