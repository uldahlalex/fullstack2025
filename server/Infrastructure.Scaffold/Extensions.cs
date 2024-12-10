using core;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

    public static class Extensions
    {
        public static IServiceCollection AddDataSource(
            this IServiceCollection services, string connStr)
        {
            var assembly = typeof(Extensions).Assembly;
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connStr)
                
            );

            
            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            return services;
        }
    }
