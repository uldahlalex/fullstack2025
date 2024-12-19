namespace Api.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttApiServices(this IServiceCollection services)
    {
        services.AddHostedService<MqttApi>(); // Change to AddHostedService
        return services;
    }

    public static WebApplication ConfigureMqttApi(this WebApplication app)
    {
        // var mqttListener = app.Services.GetRequiredService<IMqttListener>();
        // mqttListener.StartAsync(CancellationToken.None);
        return app;
    }
}