namespace Hengeler.Application.DTOs.Auth;
public class SocialUserDto
{
  public required string Username { get; set; }
  public string? Email { get; set; }
  public string? ProfilePictureUrl { get; set; }
}
