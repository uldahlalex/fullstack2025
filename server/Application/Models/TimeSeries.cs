namespace Application.Models;

public class TimeSeries
{
    public int Id { get; set; }
    public int? Datapoint { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}