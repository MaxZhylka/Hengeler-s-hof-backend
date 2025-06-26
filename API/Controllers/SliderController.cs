using Hengeler.Application.DTOs.Slider;
using Hengeler.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hengeler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SliderController(ISliderService sliderService, IConfiguration configuration) : ControllerBase
{
  private readonly ISliderService _sliderService = sliderService;
  private readonly string _adminEmails = configuration["AdminEmails"] ?? "";

  [HttpGet("{sliderId}")]
  public async Task<ActionResult<SliderDto>> GetSliderById(string sliderId)
  {
    var result = await _sliderService.GetSliderByIdAsync(sliderId);
    return Ok(result);
  }

  [Authorize]
  [HttpPost]
  public async Task<ActionResult<SliderDto>> CreateOrUpdateSlider([FromBody] CreateSliderDto dto)
  {
    var email = User.FindFirst("email")?.Value;
    if (!_adminEmails.Split(',').Contains(email))
      return Forbid();

    var result = await _sliderService.CreateOrUpdateSliderAsync(dto);
    return Ok(result);
  }
}
