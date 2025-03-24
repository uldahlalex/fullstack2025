using System.ComponentModel.DataAnnotations;

namespace Application.Models.Dtos;

public abstract class ApplicationBaseDto
{
    [Required] public abstract string EventType { get; set; } 
}