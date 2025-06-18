
using System.ComponentModel.DataAnnotations;
using Hengeler.Domain.Enums;

namespace Hengeler.Domain.Entities
{
  public class User
  {
    public Guid Id { get; set; }
    public string Username { get; set; }
    public Roles Role { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool SocialLogin { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User(Guid id, string username, string? email = null, Roles role = Roles.User, bool socialLogin = false, string? phoneNumber = null, string? passwordHash = null, string? profilePictureUrl = null)
    {
      Id = id;
      Username = username;
      Email = email;
      PhoneNumber = phoneNumber;
      Role = role;
      PasswordHash = passwordHash;
      SocialLogin = socialLogin;
      ProfilePictureUrl = profilePictureUrl;
    }

    protected User() { }

  }
}