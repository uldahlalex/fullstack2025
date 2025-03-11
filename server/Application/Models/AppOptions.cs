using System.ComponentModel.DataAnnotations;
// ReSharper disable InconsistentNaming

namespace Application.Models;

public sealed class AppOptions
{
    [Required] public string JwtSecret { get; set; } = null!;
    [Required] public string DbConnectionString { get; set; } = null!;
    public bool Seed { get; set; } = true;
    public string MQTT_BROKER_HOST { get; set; } = null!;
    public string MQTT_USERNAME { get; set; } = null!;
    public string MQTT_PASSWORD { get; set; } = null!;
    public int PORT { get; set; } = 8080;
    public int WS_PORT { get; set; } = 8181;
    public int REST_PORT { get; set; } = 5000;
}