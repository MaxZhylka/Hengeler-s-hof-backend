using Hengeler.Application.DTOs.Booking;
using Hengeler.Domain.Entities;

namespace Hengeler.Domain.Interfaces;

public interface IBookingDomainService
{
  Task<Booking> CreatePendingBookingAsync(CreateStripeSessionDto dto);
  Task<Booking> BookAsync(Guid bookingId);
  Task<List<BookingDto>> GetBookings();
  Task<bool> IsFreeAsync(string roomId, DateTime startDate, DateTime endDate);
}