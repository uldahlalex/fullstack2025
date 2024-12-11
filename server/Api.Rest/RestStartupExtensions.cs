using Scalar.AspNetCore;

namespace Api.Rest;

public static class RestStartupExtensions
{
    public static WebApplicationBuilder AddDependenciesForRestApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument();
        builder.Services.AddControllers();
        return builder;
    }

    public static WebApplication AddMiddlewareForRestApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi(options =>
            {
                options.Path = "/openapi";
            });
            app.MapScalarApiReference();
        }
        app.MapControllers();
        app.UseCors(opts => opts.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        return app;
    }
}