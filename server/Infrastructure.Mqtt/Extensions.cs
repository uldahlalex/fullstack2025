using Application.Interfaces.Infrastructure.Mqtt;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITimeSeriesPublishing, TimeSeriesPublishing>();
        return services;
    }

    // public static WebApplication AddMiddlewareForMqttInfrastructure(this WebApplication app)
    // {
    //     return app;
    // }
}