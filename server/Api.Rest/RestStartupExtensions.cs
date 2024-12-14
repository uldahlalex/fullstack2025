using Scalar.AspNetCore;

namespace Api.Rest;

public static class RestStartupExtensions
{
    public static IServiceCollection AddDependenciesForRestApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument();
        services.AddControllers();
        return services;
    }

    public static WebApplication AddMiddlewareForRestApi(this WebApplication app)
    {
        // if (!app.Environment.IsProduction()) {
            app.UseOpenApi(options => { options.Path = "/openapi/myapi.json"; });
            //To open the Scalar page, go to: http://localhost:5000/scalar/myapi
            app.MapScalarApiReference();
        // }

        app.MapControllers();
        app.UseCors(opts => opts.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        return app;
    }
}