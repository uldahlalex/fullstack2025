using Microsoft.Extensions.DependencyInjection;
using service.Interfaces;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection AddMqttInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITimeSeriesPublishing, TimeSeriesPublishing>();
        return services;
    }
}