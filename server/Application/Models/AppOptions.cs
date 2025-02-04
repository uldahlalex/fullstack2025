using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public sealed class AppOptions
{
    [Required] public required string JwtSecret { get; set; }


    [Required] public string DbConnectionString { get; set; }
    public bool RunDbInTestContainer { get; set; }
    public bool Seed { get; set; }
    public string ASPNETCORE_ENVIRONMENT { get; set; }
    public string REDIS_HOST {get; set;}
    public string REDIS_USERNAME {get; set;}
    public string REDIS_PASSWORD {get; set;}
    public string MQTT_BROKER_HOST {get; set;}
    public string MQTT_USERNAME {get; set;}
    public string MQTT_PASSWORD {get; set;}
}