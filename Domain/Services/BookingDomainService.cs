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
  public async Task<Booking> CreatePendingBookingAsync(Booking booking)
  {
    await dbContext.Bookings
    .Where(b => b.Status == BookingStatus.Pending && b.ExpiresAt != null && b.ExpiresAt < DateTime.UtcNow)
    .ExecuteDeleteAsync();

    using var transaction = await dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

    int price = await CalculatePriceAsync(booking.RoomId, booking.StartDate, booking.EndDate);
    if (booking.RoomId == "house") booking.WholeHouse = true;
    var isFree = await IsFreeAsync(booking.RoomId, booking.StartDate, booking.EndDate, booking.WholeHouse);
    if (!isFree) throw new InvalidOperationException("Room is already booked.");
    booking.Price = price;
    dbContext.Bookings.Add(booking);
    await dbContext.SaveChangesAsync();
    await transaction.CommitAsync();
    return booking;
  }

  public async Task<bool> IsFreeAsync(string roomId, DateOnly startDate, DateOnly endDate, bool wholeHouse)
  {
    if (wholeHouse)
    {
      return !await dbContext.Bookings.AnyAsync(b =>
        (b.Status == BookingStatus.Booked || b.Status == BookingStatus.Pending || b.Status == BookingStatus.ClosedByAdmin) &&
        b.StartDate < endDate &&
        b.EndDate > startDate
      );
    }
    else
    {
      return !await dbContext.Bookings.AnyAsync(b =>
        (b.Status == BookingStatus.Booked || b.Status == BookingStatus.Pending || b.Status == BookingStatus.ClosedByAdmin) &&
        (b.RoomId == roomId || b.WholeHouse) &&
        b.StartDate < endDate &&
        b.EndDate > startDate
      );
    }
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


  public async Task<List<BookingDto>> GetBookingsAsync()
  {
    await dbContext.Bookings
    .Where(b => b.Status == BookingStatus.Pending && b.ExpiresAt != null && b.ExpiresAt < DateTime.UtcNow)
    .ExecuteDeleteAsync();

    return await dbContext.Bookings
      .Where(b => b.EndDate >= DateOnly.FromDateTime(DateTime.UtcNow))
      .Include(b => b.User)
      .Select(b => new BookingDto
      {
        Id = b.Id,
        RoomId = b.RoomId,
        StartDate = b.StartDate,
        EndDate = b.EndDate,
        Status = b.Status,
        CustomerEmail = b.User!.Email,
        CustomerPhone = b.User.PhoneNumber,
        CustomerName = b.User.Username,
        StripeId = b.StripeId,
        MoreThanTwoPets = b.MoreThanTwoPets,
        UserId = b.UserId,
      })
      .ToListAsync();
  }

  public async Task<Booking> UpdateBookingStripeIdAsync(Guid bookingId, string stripeId)
  {
    if (string.IsNullOrWhiteSpace(stripeId))
      throw new ArgumentException("StripeId cannot be null or empty.", nameof(stripeId));

    var booking = await dbContext.Bookings
      .AsTracking()
      .FirstOrDefaultAsync(b => b.Id == bookingId)
      ?? throw new InvalidOperationException("Booking not found.");

    booking.StripeId = stripeId;
    await dbContext.SaveChangesAsync();
    return booking;
  }


  private async Task<int> CalculatePriceAsync(string roomId, DateOnly startDate, DateOnly endDate)
  {
    if (endDate <= startDate)
      throw new ArgumentException("End date must be after start date");

    int totalDays = (endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue)).Days;

    if (totalDays < 3)
      throw new ArgumentException("Reservation must be at least 3 days long");

    var room = await dbContext.Rooms
      .FirstOrDefaultAsync(r => r.RoomId == roomId)
      ?? throw new KeyNotFoundException($"Room with id '{roomId}' not found");

    if (room.Price is 0)
      throw new InvalidOperationException("Room price is not defined");

    int basePrice = room.Price;
    int additionalPrice = room.AdditionalPrice;

    if (totalDays <= 7)
      return totalDays * (basePrice + additionalPrice);

    return totalDays * basePrice;
  }


  public async Task DeleteBookingByIdAsync(Guid bookingId)
  {
    var rowsAffected = await dbContext.Bookings
    .Where(b => b.Id == bookingId)
    .ExecuteDeleteAsync();
  }
}
