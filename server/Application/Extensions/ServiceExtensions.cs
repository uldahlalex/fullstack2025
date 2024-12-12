using Microsoft.Extensions.DependencyInjection;
using service.Interfaces;
using service.Services;

namespace service.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IServiceLogic, ServiceLogic>();
        return services;
    }
}