namespace Application.Models.Entities;

public class Device
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Devicelog> Devicelogs { get; set; } = new List<Devicelog>();
}