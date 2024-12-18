using Application.Interfaces;
using Application.Interfaces.Infrastructure.Broadcasting;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISecurityService, SecurityService>();
        services.AddScoped<IServiceLogic, ServiceLogic>();
        return services;
    }
}