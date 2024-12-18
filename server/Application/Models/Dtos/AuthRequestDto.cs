using System.ComponentModel.DataAnnotations;

namespace Application.Models.Dtos;

public class AuthRequestDto
{
    [MinLength(3)] public string Username { get; set; }
    [MinLength(6)] public string Password { get; set; }
}