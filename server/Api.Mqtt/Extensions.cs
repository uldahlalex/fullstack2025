namespace Api.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttApiServices(this IServiceCollection services)
    {

        services.AddHostedService<MqttApi>();        
        return services;
    }

    public static WebApplication ConfigureMqttApi(this WebApplication app)
    {
        //Due to hosted service nature, we don't need to start the mqtt api explicitly
        return app;
    }
}