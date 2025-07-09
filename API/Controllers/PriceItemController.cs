using Hengeler.Application.DTOs.PriceList;
using Hengeler.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hengeler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceListController(IPriceListApplicationService service, IConfiguration configuration) : ControllerBase
{
  private readonly IPriceListApplicationService _service = service;
    private readonly string _adminEmails = configuration["AdminEmails"] ?? "";
  [HttpGet]
  public async Task<ActionResult<List<PriceItemDto>>> GetAll()
  {
    var prices = await _service.GetAllAsync();
    return Ok(prices);
  }

  [HttpPut]
  public async Task<ActionResult<List<PriceItemDto>>> ReplaceAll([FromBody] List<PriceItemDto> dtos)
  {
    var email = User.FindFirst("email")?.Value;
    if (!_adminEmails.Split(',').Contains(email))
    {
      return Forbid();
    }
    if (dtos == null || dtos.Count == 0)
      return BadRequest("List of price items is required");

    var updated = await _service.ReplaceAllAsync(dtos);
    return Ok(updated);
  }
}
