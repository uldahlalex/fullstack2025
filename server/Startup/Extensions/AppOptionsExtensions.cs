using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using service.Types;

namespace Startup.Extensions;

public static class AppOptionsExtensions
{
    public static AppOptions AddAppOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptionsWithValidateOnStart<AppOptions>()
            .Bind(builder.Configuration.GetSection(nameof(AppOptions)))
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
        return builder.Configuration.GetSection(nameof(AppOptions)).Get<AppOptions>() ??
               throw new InvalidCastException("Could not parse as AppOptions");
    }
}