using Hengeler.Application.DTOs.Auth;

namespace Hengeler.Application.Interfaces
{
  public interface IAuthService
  {
    Task<UserDto> RegisterUserAsync(CreateUserDto createUserDto);
    Task<UserDto> RegisterSocialUserAsync(SocialUserDto socialUserDto);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto> LoginAsync(LoginDto loginDto);
  }
}
