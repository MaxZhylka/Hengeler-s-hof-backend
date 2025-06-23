using Hengeler.Application.DTOs.Event;
using Hengeler.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hengeler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(IEventService eventAppService) : ControllerBase
{
  private readonly IEventService _eventAppService = eventAppService;

  [HttpPost]
  [Consumes("multipart/form-data")]
  public async Task<IActionResult> CreateEvent(
      [FromForm] EventCreateFormModel model,
      CancellationToken cancellationToken)
  {
    var dto = new EventCreateDto
    {
      UkTitle = model.UkTitle,
      EnTitle = model.EnTitle,
      DeTitle = model.DeTitle,
      UkDescription = model.UkDescription,
      EnDescription = model.EnDescription,
      DeDescription = model.DeDescription,
      Link = model.Link ?? "",
      OneDayEvent = model.OneDayEvent,
      IsActive = model.IsActive,
      StartDate = DateOnly.Parse(model.StartDate),
      EndDate = DateOnly.Parse(model.EndDate),
      StartTime = TimeOnly.Parse(model.StartTime)
    };
    var eventId = await _eventAppService.CreateEventAsync(dto, model.Image, cancellationToken);
    return Ok(eventId);
  }

  [HttpPut("{id:guid}")]
  [Consumes("multipart/form-data")]
  public async Task<IActionResult> UpdateEvent(
      Guid id,
      [FromForm] EventUpdateFormModel model,
      CancellationToken cancellationToken)
  {

    var dto = new EventUpdateDto
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
      ImageUrl = model.ImageUrl,
      Link = model.Link,
      OneDayEvent = model.OneDayEvent,
      IsActive = model.IsActive,
      StartDate = DateOnly.Parse(model.StartDate),
      EndDate = DateOnly.Parse(model.EndDate),
      StartTime = TimeOnly.Parse(model.StartTime)
    };

    await _eventAppService.UpdateEventAsync(dto, model.Image, cancellationToken);
    return NoContent();
  }

  [HttpGet]
  public async Task<IActionResult> GetAllEvents()
  {
    var events = await _eventAppService.GetAllEventsAsync();
    return Ok(events);
  }

  [HttpPost("/api/Events/setIsActive")]
  public async Task<IActionResult> SetIsActive(
      [FromQuery] Guid id,
      [FromQuery] bool isActive,
      CancellationToken cancellationToken)
  {
    await _eventAppService.SetEventActiveStatusAsync(id, isActive, cancellationToken);
    return Ok();
  }

  [HttpGet("/api/Events/Active")]
  public async Task<IActionResult> GetAllActiveEvents(CancellationToken cancellationToken)
  {
    var events = await _eventAppService.GetAllActiveEventsAsync(cancellationToken);
    return Ok(events);
  }

  [HttpDelete]
  public async Task<IActionResult> DeleteEventById([FromQuery] Guid id, CancellationToken cancellationToken)
  {
    await _eventAppService.DeleteEventByIdAsync(id, cancellationToken);
    return Ok();
  }
}
