using Api.Rest.Controllers;
using Api.Rest.Middleware;
using Scalar.AspNetCore;

namespace Api.Rest;

public static class RestStartupExtensions
{
    public static IServiceCollection AddDependenciesForRestApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddOpenApiDocument();
        var controllersAssembly = typeof(MyController).Assembly;

        services.AddControllers().AddApplicationPart(controllersAssembly);
        return services;
    }

    public static WebApplication AddMiddlewareForRestApi(this WebApplication app)
    {
        app.UseExceptionHandler();

        app.UseOpenApi(options => { options.Path = "/openapi/myapi.json"; });
        //To open the Scalar page, go to: http://localhost:5000/scalar/myapi
        app.MapScalarApiReference();

app.MapControllers();
        app.UseCors(opts => opts.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        return app;
    }
}