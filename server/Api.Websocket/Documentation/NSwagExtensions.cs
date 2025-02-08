
using System.Text.Json;
using Api.Websocket.Events;
using Namotion.Reflection;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using WebSocketBoilerplate;

namespace Api.Websocket.Documentation;

public sealed class AddAllDerivedTypesProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        // Get all assemblies in the current application domain
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Find all types that inherit from BaseDto
        var derivedTypes = assemblies
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
                t != typeof(BaseDto) && // Exclude BaseDto itself
                !t.IsAbstract && // Exclude abstract classes
                typeof(BaseDto).IsAssignableFrom(t)) // Check if type inherits from BaseDto
            .ToList();

        // Generate schema for each derived type
        foreach (var type in derivedTypes)
        {
            context.SchemaGenerator.Generate(type.ToContextualType(), context.SchemaResolver);
        }
    }
}


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
        foreach (var typeName in derivedTypeNames)
        {
            schema.Enumeration.Add(typeName);
        }

        context.Document.Definitions["StringConstants"] = schema;
    }
}


