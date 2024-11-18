
using AsyncApi.Net.Generator;
using AsyncApi.Net.Generator.AsyncApiSchema.v2;
using realtimeapi.Hubs;


namespace realtimeapi;

public static class RealtimeStartupExtensions
{
    public static WebApplicationBuilder AddDependenciesForRealtimeApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();
        builder.Services.AddAsyncApiSchemaGeneration(o =>
        {
            o.AssemblyMarkerTypes = new[] { typeof(ChatHub) };
            o.Middleware.UiTitle = "API Documentation";
            o.AsyncApi = new AsyncApiDocument
            {
                Info = new Info { Title = "My application", Description = "Fullstack 2025"}
            }; 
        });
        return builder;
    }
    
    public static WebApplication AddMiddlewareForRealtimeApi(this WebApplication app)
    {
        app.UseRouting();
         app.MapHub<ChatHub>("/chatHub");
         app.MapAsyncApiDocuments();
         app.MapAsyncApiUi();

        return app;
    }
}