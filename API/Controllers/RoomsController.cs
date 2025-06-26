using Hengeler.Application.DTOs.Room;
using Hengeler.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hengeler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController(IRoomService roomAppService, IConfiguration configuration) : ControllerBase
{
  private readonly IRoomService _roomAppService = roomAppService;
  
  private readonly string _adminEmails = configuration["AdminEmails"] ?? "";

  [HttpGet("{id}")]
  public async Task<ActionResult<RoomDto>> GetRoomById(string id)
  {
    try
    {
      var room = await _roomAppService.GetRoomByIdAsync(id);
      return Ok(room);
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { message = ex.Message });
    }
  }

  [Authorize]
  [HttpPost]
  public async Task<ActionResult<RoomDto>> CreateOrUpdateRoom([FromBody] CreateOrUpdateRoomDto dto)
  {
    var email = User.FindFirst("email")?.Value;
    if (!_adminEmails.Split(',').Contains(email))
    {
      Console.WriteLine(_adminEmails);
      Console.WriteLine(email);
      return Forbid();
    }
    var result = await _roomAppService.CreateOrUpdateRoomAsync(dto);
    return Ok(result);
  }
}
