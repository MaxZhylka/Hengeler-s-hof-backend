using Hengeler.Application.DTOs.Auth;
using Hengeler.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hengeler.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
  private readonly IAuthService _authService = authService;

  [HttpPost("createUser")]
  public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var userDto = await _authService.RegisterUserAsync(createUserDto);
    return CreatedAtAction(nameof(GetUser), new { userId = userDto.Id }, userDto);
  }

[HttpGet("getUser")]
public async Task<IActionResult> GetUser([FromQuery] Guid userId)
{
  var userDto = await _authService.GetUserByIdAsync(userId);
  if (userDto == null)
    return NotFound();

  return Ok(userDto);
}

[HttpGet("getUserByEmail")]
public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
{
  var userDto = await _authService.GetUserByEmailAsync(email);
  if (userDto == null)
    return NotFound();

  return Ok(userDto);
}

  [HttpPost("socialCreate")]
  public async Task<IActionResult> SocialCreate([FromBody] SocialUserDto socialUserDto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var userDto = await _authService.RegisterSocialUserAsync(socialUserDto);
    return CreatedAtAction(nameof(GetUser), new { userId = userDto.Id }, userDto);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var userDto = await _authService.LoginAsync(loginDto);
    return Ok(userDto);
  }
}
