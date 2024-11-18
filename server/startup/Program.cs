using realtimeapi;
using restapi;

public static class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.AddDependenciesForRestApi();
        builder.AddDependenciesForRealtimeApi();
        
        
        var app = builder.Build();

        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();
        app.UseCors(opts => opts.AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(_ => true));

        app.Run();
    }
}