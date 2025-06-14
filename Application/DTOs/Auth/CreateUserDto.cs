namespace Hengeler.Application.DTOs.Auth;

public class CreateUserDto
{
  public required string Username { get; set; }
  public required string Email { get; set; }
  public required string Password { get; set; }
  public string? PhoneNumber { get; set; }
}
