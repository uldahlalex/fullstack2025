using System;
using System.Collections.Generic;

namespace Application.Models.Entities;

public partial class Devicelog
{
    public string Id { get; set; } = null!;

    public string Deviceid { get; set; } = null!;

    public double Value { get; set; }

    public DateTime Timestamp { get; set; }

    public string Unit { get; set; } = null!;
}
