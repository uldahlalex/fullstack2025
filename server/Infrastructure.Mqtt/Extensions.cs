using Application.Interfaces.Infrastructure.TimeeSeries;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection AddMqttInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITimeSeriesPublishing, TimeSeriesPublishing>();
        return services;
    }

    public static WebApplication AddMiddlewareForMqttInfrastructure(this WebApplication app)
    {
        
    }
}