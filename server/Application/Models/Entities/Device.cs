using System;
using System.Collections.Generic;

namespace Application.Models.Entities;

public partial class Device
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Devicelog> Devicelogs { get; set; } = new List<Devicelog>();
}
