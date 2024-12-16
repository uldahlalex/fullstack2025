using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.Options;
using service;

namespace Startup.Extensions;

public static class AppOptionsExtensions
{
    public static AppOptions AddAppOptions(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment env) 
    {
        services.AddOptionsWithValidateOnStart<AppOptions>()
            .Bind(configuration.GetSection(nameof(AppOptions)))
            .PostConfigure(options => 
            {
                options.ASPNETCORE_ENVIRONMENT = env.EnvironmentName;
            })
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
        
        options.ASPNETCORE_ENVIRONMENT = env.EnvironmentName;
        
        Console.WriteLine("AppOptions: " + JsonSerializer.Serialize(options));
        return options;
    }
}