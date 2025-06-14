using Hengeler.Domain.Enums;

namespace Hengeler.Application.DTOs.Auth;

public class UserDto
{
  public Guid Id { get; set; }
  public required string Username { get; set; }
  public string? Email { get; set; }
  public string? PhoneNumber { get; set; }
  public string? ProfilePictureUrl { get; set; }
  public DateTime CreatedAt { get; set; }
  public Roles Role { get; set; }
}
