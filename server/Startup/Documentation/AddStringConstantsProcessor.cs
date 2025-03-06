using Application.Models.Dtos;
using Infrastructure.Mqtt.Interfaces;
using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using WebSocketBoilerplate;

namespace Startup.Documentation;

/// <summary>
///     I want nswag to include event type names like "ClientWantsToDoX" as simple string constants
/// </summary>
public sealed class AddStringConstantsProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var derivedTypeNames = assemblies
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            })
            .Where(t =>
                (t != typeof(BaseDto) &&
                 !t.IsAbstract &&
                 typeof(BaseDto).IsAssignableFrom(t)) ||
                (t != typeof(ApplicationBaseDto) &&
                 !t.IsAbstract &&
                 typeof(ApplicationBaseDto).IsAssignableFrom(t)) ||
                (t != typeof(IMqttEventDto) &&
                 !t.IsAbstract &&
                 typeof(IMqttEventDto).IsAssignableFrom(t)) ||
                t == typeof(AdminWantsToChangePreferencesForDeviceDto)
            )
            .Select(t => t.Name)
            .ToArray();

        var schema = new JsonSchema
        {
            Type = JsonObjectType.String,
            Description = "Available eventType constants"
        };

        foreach (var typeName in derivedTypeNames) schema.Enumeration.Add(typeName);

        context.Document.Definitions["StringConstants"] = schema;
    }
}