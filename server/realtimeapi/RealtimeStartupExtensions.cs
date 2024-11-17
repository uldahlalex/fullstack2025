
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
            o.Middleware.UiTitle = "Streetlights API";
            o.AsyncApi = new AsyncApiDocument
            {
                Info = new Info
                {
                    Title = "Streetlights API",
                    Description = "The Smartylighting Streetlights API allows you to remotely manage the city lights.",
                    License = new License
                    {
                        Name = "Apache 2.0",
                        Url = "https://www.apache.org/licenses/LICENSE-2.0"
                    }
                },
                Servers = new()
                {
                    ["mosquitto"] = new Server
                    {
                        Url = "test.mosquitto.org",
                        Protocol = "mqtt",
                    },
                    ["webapi"] = new Server
                    {
                        Url = "localhost:5000",
                        Protocol = "http",
                    },
                },
            };
            o.AssemblyMarkerTypes = new[] { typeof(ChatHub) }; // add assemply marker
            o.AsyncApi = new AsyncApiDocument { Info = new Info { Title = "My application" }}; // introduce your application
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