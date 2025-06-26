using Hengeler.Application.DTOs.Slides;
using Hengeler.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hengeler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SlidesController(ISlideService slideService, IConfiguration configuration) : ControllerBase
{
  private readonly ISlideService _slideService = slideService;
  private readonly string _adminEmails = configuration["AdminEmails"] ?? "";

  [Authorize]
  [HttpPost]
  [Consumes("multipart/form-data")]
  public async Task<IActionResult> CreateSlide([FromForm] SlideCreateFormModel model, CancellationToken cancellationToken)
  {
    var email = User.FindFirst("email")?.Value;
    if (!_adminEmails.Split(',').Contains(email))
      return Forbid();

    var dto = new SlideCreateDto
    {
      UkTitle = model.UkTitle,
      EnTitle = model.EnTitle,
      DeTitle = model.DeTitle,
      UkDescription = model.UkDescription,
      EnDescription = model.EnDescription,
      DeDescription = model.DeDescription,
      Price = model.Price
    };

    var slideId = await _slideService.CreateSlideAsync(dto, model.Image, cancellationToken);
    return Ok(slideId);
  }

  [Authorize]
  [HttpPut("{id:guid}")]
  [Consumes("multipart/form-data")]
  public async Task<IActionResult> UpdateSlide(Guid id, [FromForm] SlideUpdateFormModel model, CancellationToken cancellationToken)
  {
    var email = User.FindFirst("email")?.Value;
    if (!_adminEmails.Split(',').Contains(email))
      return Forbid();

    var dto = new SlideUpdateDto
    {
      Id = Guid.Parse(model.Id),
      TitleKey = Guid.Parse(model.TitleKey),
      DescriptionKey = Guid.Parse(model.DescriptionKey),
      UkTitle = model.UkTitle,
      EnTitle = model.EnTitle,
      DeTitle = model.DeTitle,
      UkDescription = model.UkDescription,
      EnDescription = model.EnDescription,
      DeDescription = model.DeDescription,
      Price = model.Price
    };

    await _slideService.UpdateSlideAsync(dto, model.Image, cancellationToken);
    return NoContent();
  }

  [HttpGet]
  public async Task<IActionResult> GetAllSlides()
  {
    var slides = await _slideService.GetAllSlidesAsync();
    return Ok(slides);
  }

  [Authorize]
  [HttpDelete]
  public async Task<IActionResult> DeleteSlide([FromQuery] Guid id, CancellationToken cancellationToken)
  {
    var email = User.FindFirst("email")?.Value;
    if (!_adminEmails.Split(',').Contains(email))
      return Forbid();

    await _slideService.DeleteSlideAsync(id, cancellationToken);
    return Ok();
  }
}
