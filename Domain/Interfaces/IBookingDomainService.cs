using Hengeler.Application.DTOs.Booking;
using Hengeler.Domain.Entities;

namespace Hengeler.Domain.Interfaces;

public interface IBookingDomainService
{
  Task<Booking> CreatePendingBookingAsync(Booking dto);
  Task<Booking> BookAsync(Guid bookingId);
  Task<List<BookingDto>> GetBookingsAsync();
  Task<bool> IsFreeAsync(string roomId, DateTime startDate, DateTime endDate, bool wholeHouse);
}