using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Generation;

namespace Startup.Extensions;

public static class GenerateTypescriptClient
{
    public static async Task GenerateTypeScriptClient(this WebApplication app, string docName)
    {
        var document = await app.Services.GetRequiredService<IOpenApiDocumentGenerator>()
            .GenerateAsync(docName);

        var settings = new TypeScriptClientGeneratorSettings
        {
            Template = TypeScriptTemplate.Axios,
            TypeScriptGeneratorSettings =
            {
                TypeStyle = TypeScriptTypeStyle.Interface,
                DateTimeType = TypeScriptDateTimeType.Date,
                NullValue = TypeScriptNullValue.Undefined,
                TypeScriptVersion = 4.3m,
                GenerateCloneMethod = false,
            }
        };

        var generator = new TypeScriptClientGenerator(document, settings);
        var code = generator.GenerateFile();

        // Define the output path - adjust as needed
        
        string outputPath = Path.Combine(Directory.GetCurrentDirectory()+"/../../client/src/generated-client.ts");
        
        // Ensure directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        // Write the file
        await File.WriteAllTextAsync(outputPath, code);

        Console.WriteLine($"TypeScript client generated at: {outputPath}");
    }
}