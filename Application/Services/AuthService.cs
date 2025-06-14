using Hengeler.Application.DTOs.Auth;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Application.Interfaces;

namespace Hengeler.Application.Services
{
  public class AuthService(IAuthDomainService authDomainService) : IAuthService
  {
    private readonly IAuthDomainService _authDomainService = authDomainService;

    public async Task<UserDto> RegisterUserAsync(CreateUserDto createUserDto)
    {

      var user = await _authDomainService.CreateUserAsync(
        createUserDto.Username,
        createUserDto.Email,
        createUserDto.Password,
        createUserDto.PhoneNumber);

      return MapToUserDto(user);
    }

    public async Task<UserDto> RegisterSocialUserAsync(SocialUserDto socialUserDto)
    {
      var user = await _authDomainService.CreateSocialUserAsync(
        socialUserDto.Username,
        socialUserDto.Email,
        socialUserDto.ProfilePictureUrl);

      return MapToUserDto(user);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
      var user = await _authDomainService.GetUserByIdAsync(userId);
      return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
      var user = await _authDomainService.GetUserByEmailAsync(email);
      return user == null ? null : MapToUserDto(user);
    }

    public async Task<UserDto> LoginAsync(LoginDto loginDto)
    {
      var user = await _authDomainService.LoginAsync(loginDto.Email, loginDto.Password);
      return MapToUserDto(user);
    }

    private static UserDto MapToUserDto(User user) =>
      new()
      {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        ProfilePictureUrl = user.ProfilePictureUrl,
        CreatedAt = user.CreatedAt,
        Role = user.Role
      };
  }
}
