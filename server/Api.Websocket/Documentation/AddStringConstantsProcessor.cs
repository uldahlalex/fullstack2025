using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using WebSocketBoilerplate;

namespace Api.Websocket.Documentation;

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
                t != typeof(BaseDto) &&
                !t.IsAbstract &&
                typeof(BaseDto).IsAssignableFrom(t))
            .Select(t => t.Name)
            .ToArray();

        var schema = new JsonSchema
        {
            Type = JsonObjectType.String,
            Description = "Available eventType constants"
        };

        // Add each type name to the Enumeration collection
        foreach (var typeName in derivedTypeNames) schema.Enumeration.Add(typeName);

        context.Document.Definitions["StringConstants"] = schema;
    }
}