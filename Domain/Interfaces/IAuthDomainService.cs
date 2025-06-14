using Hengeler.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Hengeler.Domain.Interfaces
{
  public interface IAuthDomainService
  {
    Task<User> CreateUserAsync(string username, string email, string password, string? phoneNumber = null);

    Task<User> CreateSocialUserAsync(string username, string? email, string? profilePictureUrl = null);

    Task<User?> GetUserByIdAsync(Guid userId);

    Task<User?> GetUserByEmailAsync(string email);

    Task<User> LoginAsync(string email, string password);

    Task<bool> VerifyPasswordAsync(string username, string plainPassword);
  }
}
