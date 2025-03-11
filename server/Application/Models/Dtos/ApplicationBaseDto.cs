using System.ComponentModel.DataAnnotations;

namespace Application.Models.Dtos;

public abstract class ApplicationBaseDto
{
    [Required] public string EventType { get; set; } = null!;
}