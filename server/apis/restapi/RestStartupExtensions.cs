namespace restapi;

public static class RestStartupExtensions
{
    public static WebApplicationBuilder AddDependenciesForRestApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        return builder;
    }

    public static WebApplication AddMiddlewareForRestApi(this WebApplication app)
    {
        app.MapControllers();
        app.UseCors(opts => opts.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        return app;
    } 
}