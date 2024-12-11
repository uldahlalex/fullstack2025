using infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddDataSource(
        this IServiceCollection services, string connStr)
    {
        var assembly = typeof(Extensions).Assembly;
            
        services.AddDbContext<MyDbContext>(options =>
            options.UseNpgsql(connStr)
                
        );

            
        using var scope = services.BuildServiceProvider().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        context.Database.EnsureCreated();

        return services;
    }
}