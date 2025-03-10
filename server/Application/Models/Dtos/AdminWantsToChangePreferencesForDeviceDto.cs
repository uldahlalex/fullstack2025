using System.ComponentModel.DataAnnotations;

namespace Application.Models.Dtos;

public class AdminWantsToChangePreferencesForDeviceDto
{
    [Required]
    public string DeviceId { get; set; }
    [Required]
    public int IntervalMilliseconds { get; set; }
    [Required]
    public string Unit { get; set; }
}