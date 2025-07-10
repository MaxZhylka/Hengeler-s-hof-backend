using System.ComponentModel.DataAnnotations;

namespace Hengeler.Application.DTOs.Auth;

public class CreateUserDto
{
  [Required, MaxLength(200)]
  public required string Username { get; set; }
  [Required, EmailAddress, MaxLength(100)]
  public required string Email { get; set; }
  [Required, MinLength(8), MaxLength(100)]
  public required string Password { get; set; }
  [Phone, MaxLength(15)]
  public string? PhoneNumber { get; set; }
}
