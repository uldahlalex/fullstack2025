namespace Startup.Tests.TestUtils;

public class ApiTestBaseConfig
{
    /// <summary>
    ///     Defaults to false
    /// </summary>
    public bool MockRelationalDatabase { get; set; } = false;

    /// <summary>
    ///     Defaults to false
    /// </summary>
    public bool MockMqtt { get; set; } = false;

    /// <summary>
    ///     Defaults to false
    /// </summary>
    public bool MockWebSocketService { get; set; } = false;

    /// <summary>
    ///     Defaults to true
    /// </summary>
    public bool MockProxyConfig { get; set; } = true;

    /// <summary>
    ///     Defaults to true
    /// </summary>
    public bool UseCustomSeeder { get; set; } = true;
}