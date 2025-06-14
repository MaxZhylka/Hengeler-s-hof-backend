using System.Security.Cryptography;
using System.Text;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Enums;
using Hengeler.Domain.Interfaces;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Domain.Services;

public class AuthDomainService(AppDbContext context) : IAuthDomainService
{
  private readonly AppDbContext _context = context;

  public async Task<User> CreateUserAsync(string username, string email, string password, string? phoneNumber = null)
  {
    if (string.IsNullOrWhiteSpace(password))
      throw new ArgumentException("Password cannot be empty.");

    if (await _context.Users.AnyAsync(u => u.Email == email))
      throw new InvalidOperationException("Email is already taken.");

    var hash = HashPassword(password);

    var user = new User(
      id: Guid.NewGuid(),
      username: username,
      email: email,
      role: Roles.User,
      socialLogin: false,
      phoneNumber: phoneNumber,
      passwordHash: hash,
      profilePictureUrl: null
    );

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return user;
  }

public async Task<User> CreateSocialUserAsync(string username, string? email, string? profilePictureUrl = null)
{
    if (string.IsNullOrWhiteSpace(email))
        throw new ArgumentException("Email must be provided", nameof(email));

    var existingUser = await _context.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Email == email);

    if (existingUser != null)
    {
        if (existingUser.ProfilePictureUrl != profilePictureUrl)
        {
            var updatedUser = new User(
                id: existingUser.Id,
                username: existingUser.Username,
                email: existingUser.Email,
                role: existingUser.Role,
                socialLogin: existingUser.SocialLogin,
                phoneNumber: existingUser.PhoneNumber,
                passwordHash: existingUser.PasswordHash,
                profilePictureUrl: profilePictureUrl
            );

            _context.Users.Update(updatedUser);
            await _context.SaveChangesAsync();

            return updatedUser;
        }

        return existingUser;
    }

    var newUser = new User(
        id: Guid.NewGuid(),
        username: username,
        email: email,
        role: Roles.User,
        socialLogin: true,
        phoneNumber: null,
        passwordHash: null,
        profilePictureUrl: profilePictureUrl
    );

    _context.Users.Add(newUser);
    await _context.SaveChangesAsync();

    return newUser;
}

  public async Task<User?> GetUserByIdAsync(Guid userId)
  {
    return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
  }

  public async Task<User?> GetUserByEmailAsync(string email)
  {
    return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
  }

  public async Task<User> LoginAsync(string email, string password)
  {
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
      throw new ArgumentException("Email and password must be provided.");

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new InvalidOperationException("Invalid email or password.");

    if (user.SocialLogin)
      throw new InvalidOperationException("User has logged in via social login and cannot use password authentication.");

    var hash = HashPassword(password);

    if (hash != user.PasswordHash)
      throw new InvalidOperationException("Invalid email or password.");

    return user;
  }

  public async Task<bool> VerifyPasswordAsync(string username, string plainPassword)
  {
    var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
    if (user == null || user.PasswordHash == null)
      return false;

    var hash = HashPassword(plainPassword);
    return hash == user.PasswordHash;
  }

  private string HashPassword(string password)
  {
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(password);
    var hashBytes = sha256.ComputeHash(bytes);
    return Convert.ToBase64String(hashBytes);
  }
}
