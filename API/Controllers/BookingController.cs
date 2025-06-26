
using Hengeler.Application.DTOs.Booking;
using Hengeler.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hengeler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController(IBookingService bookingService, IConfiguration configuration) : ControllerBase
{
  private readonly IBookingService _bookingService = bookingService;
  private readonly string _adminEmails = configuration["AdminEmails"] ?? "";

  [Authorize]
  [HttpPost("create-stripe-session")]
  public async Task<IActionResult> CreateStripeSession([FromBody] CreateStripeSessionDto createStripeSessionDto)
  {


    if (createStripeSessionDto == null)
    {
      return BadRequest("Invalid request data.");
    }

    var session = await _bookingService.CreateStripeSessionAsync(createStripeSessionDto);
    return Ok(new { sessionId = session });
  }

  [HttpPost("stripe/webhook")]
  public async Task<IActionResult> StripeWebhook()
  {
    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
    await _bookingService.HandleStripeWebhookAsync(json, HttpContext.Request);
    return Ok();
  }

  [HttpGet("get-bookings")]
  public async Task<IActionResult> GetBookings()
  {
    List<BookingDto> bookings = await _bookingService.GetBookingsAsync();
    return Ok(bookings);
  }

  [Authorize]
  [HttpPost("book-by-admin")]
  public async Task<IActionResult> BookByAdmin([FromBody] CreateAdminBookingDto createAdminBookingDto)
  {
    var email = User.FindFirst("email")?.Value;

    if (!_adminEmails.Split(',').Contains(email))
    {
      return Forbid();
    }

    await _bookingService.BookByAdminAsync(createAdminBookingDto);
    return Ok();
  }

  [Authorize]
  [HttpDelete]
  public async Task<IActionResult> DeleteBooking([FromQuery] Guid id)
  {
    var email = User.FindFirst("email")?.Value;

    if (!_adminEmails.Split(',').Contains(email))
    {
      return Forbid();
    }

    await _bookingService.DeleteBookingByIdAsync(id);
    return Ok();
  }
}