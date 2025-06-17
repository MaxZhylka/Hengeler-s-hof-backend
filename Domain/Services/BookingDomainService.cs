using Hengeler.Application.DTOs.Booking;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Domain.Services;

public class BookingDomainService(
  AppDbContext dbContext
) : IBookingDomainService
{
  public async Task<Booking> CreatePendingBookingAsync(CreateStripeSessionDto dto)
  {
    await dbContext.Bookings
.Where(b => b.Status == BookingStatus.Pending && b.ExpiresAt != null && b.ExpiresAt < DateTime.UtcNow)
.ExecuteDeleteAsync();

    using var transaction = await dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

    var isFree = await IsFreeAsync(dto.RoomId, dto.StartDate, dto.EndDate);
    if (!isFree) throw new InvalidOperationException("Room is already booked.");

    var booking = new Booking(
      price: dto.Price,
      numberOfDays: dto.NumberOfDays,
      roomId: dto.RoomId,
      userId: dto.UserId,
      status: BookingStatus.Pending,
      startDate: dto.StartDate,
      endDate: dto.EndDate,
      moreThanTwoPats: dto.MoreThanTwoPats,
      expiresAt: DateTime.UtcNow.AddMinutes(30)
    );

    dbContext.Bookings.Add(booking);
    await dbContext.SaveChangesAsync();
    await transaction.CommitAsync();
    return booking;
  }
  public async Task<bool> IsFreeAsync(string roomId, DateTime startDate, DateTime endDate)
  {
    return !await dbContext.Bookings
      .AnyAsync(b =>
        (b.Status == BookingStatus.Booked || b.Status == BookingStatus.Pending || b.Status == BookingStatus.ClosedByAdmin) &&
        (b.RoomId == roomId || b.WholeHouse) &&
        b.StartDate < endDate &&
        b.EndDate > startDate
      );
  }

  public async Task<Booking> BookAsync(Guid bookingId)
  {
    using var transaction = await dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

    var pendingBooking = await dbContext.Bookings
      .AsTracking()
      .FirstOrDefaultAsync(b => b.Status == BookingStatus.Pending && b.Id == bookingId && b.ExpiresAt > DateTime.UtcNow) ??
      throw new InvalidOperationException("Booking not found or expired.");

    var isFree = !await dbContext.Bookings.AnyAsync(b =>
    b.Status == BookingStatus.Booked &&
    (b.RoomId == pendingBooking.RoomId || b.WholeHouse) &&
    b.StartDate < pendingBooking.EndDate &&
    b.EndDate > pendingBooking.StartDate);

    if (!isFree)
    {
      throw new InvalidOperationException("Room is already booked.");
    }

    pendingBooking.Status = BookingStatus.Booked;
    pendingBooking.ExpiresAt = null;

    await dbContext.SaveChangesAsync();
    await transaction.CommitAsync();

    return pendingBooking;
  }


  public async Task<List<BookingDto>> GetBookings()
  {
    return await dbContext.Bookings
      .Where(b => b.EndDate >= DateTime.UtcNow)
      .Include(b => b.User)
      .Select(b => new BookingDto
      {
        Id = b.Id,
        RoomId = b.RoomId,
        StartDate = b.StartDate,
        EndDate = b.EndDate,
        Status = b.Status.ToString(),
        StripeSessionId = null,
        PaymentIntentId = null,
        CustomerEmail = b.User.Email
      })
      .ToListAsync();
  }
}
