using core;
using Microsoft.Extensions.DependencyInjection;

namespace service;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IServiceLogic, ServiceLogic>();
        return services;
    }
}