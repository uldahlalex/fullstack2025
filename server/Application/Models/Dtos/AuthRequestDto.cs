using System.ComponentModel.DataAnnotations;

namespace Application.Models.Dtos;

public class AuthRequestDto
{
    [MinLength(3)] public string Email { get; set; }
    [MinLength(4)] public string Password { get; set; }
}