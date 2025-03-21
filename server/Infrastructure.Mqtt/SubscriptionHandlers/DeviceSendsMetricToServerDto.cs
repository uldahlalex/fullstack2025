using Core.Domain.Entities;

namespace Infrastructure.Mqtt.SubscriptionHandlers;

public class DeviceSendsMetricToServerDto
{
    public string Unit { get; set; } = null!;
    public string DeviceId { get; set; } = null!;
    public double Value { get; set; }

    public Devicelog ToDeviceLog()
    {
        var result = new Devicelog
        {
            Unit = Unit,
            Value = Value,
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Deviceid = DeviceId
        };
        return result;
    }
}