using Application.Models.Dtos;
using Infrastructure.Mqtt.Interfaces;
using Namotion.Reflection;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using WebSocketBoilerplate;

namespace Startup.Documentation;

/// <summary>
///     I want nswag to generate schemas for all derived types of BaseDto
/// </summary>
public sealed class AddAllDerivedTypesProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

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
                t != typeof(BaseDto) &&
                !t.IsAbstract &&
                typeof(BaseDto).IsAssignableFrom(t) ||
                t != typeof(ApplicationBaseDto) &&
                !t.IsAbstract &&
                typeof(ApplicationBaseDto).IsAssignableFrom(t) ||
                t != typeof(IMqttEventDto) &&
                !t.IsAbstract &&
                typeof(IMqttEventDto).IsAssignableFrom(t)
            )
            .ToList();

        foreach (var type in derivedTypes)
            context.SchemaGenerator.Generate(type.ToContextualType(), context.SchemaResolver);
    }
}