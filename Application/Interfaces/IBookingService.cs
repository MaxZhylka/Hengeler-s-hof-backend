
using Hengeler.Application.DTOs.Booking;

namespace Hengeler.Application.Interfaces;

public interface IBookingService
{
  public Task<string> CreateStripeSessionAsync(CreateStripeSessionDto createStripeSessionDto);

  public Task HandleStripeWebhookAsync(string json, HttpRequest request);

  public Task<List<BookingDto>> GetBookingsAsync();
}
