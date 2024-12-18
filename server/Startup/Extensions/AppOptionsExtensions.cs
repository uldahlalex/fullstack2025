using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Application.Models;
using Microsoft.Extensions.Options;

namespace Startup.Extensions;

public static class AppOptionsExtensions
{
    public static AppOptions AddAppOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment env)
    {
        // First try to get from AppOptions configuration
        var configEnvironment = configuration.GetValue<string>($"{nameof(AppOptions)}:ASPNETCORE_ENVIRONMENT");
        Console.WriteLine($"Environment from config: {configEnvironment}");

        var environment = configEnvironment // First priority: AppOptions configuration
                          ?? Environment.GetEnvironmentVariable(
                              "ASPNETCORE_ENVIRONMENT") // Second priority: Environment variable
                          ?? env.EnvironmentName // Third priority: WebHostEnvironment
                          ?? "Development"; // Last resort fallback

        Console.WriteLine($"Selected environment: {environment}");

        // Force all environment indicators to match
        env.EnvironmentName = environment;
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

        services.AddOptionsWithValidateOnStart<AppOptions>()
            .Bind(configuration.GetSection(nameof(AppOptions)))
            .PostConfigure(options => { options.AspnetcoreEnvironment = environment; })
            .ValidateDataAnnotations()
            .Validate(options =>
                {
                    var context = new ValidationContext(options);
                    var validationResults = new List<ValidationResult>();
                    var isValid = Validator.TryValidateObject(options, context, validationResults, true);

                    if (!isValid)
                    {
                        var errors = validationResults.Select(r => r.ErrorMessage!);
                        throw new OptionsValidationException(nameof(AppOptions), typeof(AppOptions), errors);
                    }

                    return isValid;
                }, $"{nameof(AppOptions)} validation failed");

        var options = configuration.GetSection(nameof(AppOptions)).Get<AppOptions>() ??
                      throw new InvalidCastException("Could not parse as AppOptions");

        options.AspnetcoreEnvironment = environment;

        Console.WriteLine("Final AppOptions: " + JsonSerializer.Serialize(options));
        return options;
    }
}