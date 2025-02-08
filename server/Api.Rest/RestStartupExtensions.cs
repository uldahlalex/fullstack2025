using Api.Rest.Controllers;
using Api.Rest.Middleware;
using Scalar.AspNetCore;

namespace Api.Rest;

public static class RestStartupExtensions
{
    public static IServiceCollection RegisterRestApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

  
        var controllersAssembly = typeof(MyController).Assembly;

        services.AddControllers().AddApplicationPart(controllersAssembly);
        return services;
    }

    public static WebApplication ConfigureRestApi(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.MapControllers();
        app.UseCors(opts => opts.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        return app;
    }
}