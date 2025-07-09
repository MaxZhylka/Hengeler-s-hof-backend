using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hengeler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeaturesController(IFeatureService featureService, IConfiguration configuration) : ControllerBase
{
  private readonly IFeatureService _featureService = featureService;
    private readonly string _adminEmails = configuration["AdminEmails"] ?? "";

  [HttpGet]
  public async Task<ActionResult<List<Feature>>> GetAll()
  {
    var features = await _featureService.GetAllAsync();
    return Ok(features);
  }

  [HttpPut]
  public async Task<IActionResult> Update([FromBody] Feature dto)
  {
    var email = User.FindFirst("email")?.Value;
    if (!_adminEmails.Split(',').Contains(email))
    {
      return Forbid();
    }
    var success = await _featureService.UpdateAsync(dto);

    if (!success)
      return NotFound();

    return NoContent();
  }
}
