
using Hengeler.Application.DTOs.Booking;
using Hengeler.Application.Interfaces;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace Hengeler.Application.Services;

public class BookingService(string stripeApiKey, string successUrl, string cancelUrl, string webhookSecret, IEmailDomainService emailService, IAuthDomainService authService, IBookingDomainService bookingDomainService) : IBookingService
{
  private readonly string _stripeApiKey = stripeApiKey;
  private readonly string _successUrl = successUrl;
  private readonly string _cancelUrl = cancelUrl;
  private readonly string _webhookSecret = webhookSecret;
  private readonly IAuthDomainService _authService = authService;
  private readonly IEmailDomainService _emailService = emailService;

  private readonly IBookingDomainService _bookingDomainService = bookingDomainService;
  public async Task<string> CreateStripeSessionAsync(CreateStripeSessionDto createStripeSessionDto)
  {

    var booking = await _bookingDomainService.CreatePendingBookingAsync(new Booking
    (
      createStripeSessionDto.Price,
      createStripeSessionDto.NumberOfDays,
      createStripeSessionDto.RoomId,
      createStripeSessionDto.UserId,
      BookingStatus.Pending,
      createStripeSessionDto.StartDate,
      createStripeSessionDto.EndDate,
      createStripeSessionDto.MoreThanTwoPets,
      DateTime.UtcNow.AddMinutes(30),
      createStripeSessionDto.WholeHouse
    ));
    try
    {
      StripeConfiguration.ApiKey = _stripeApiKey;
      var options = new SessionCreateOptions
      {
        PaymentMethodTypes = ["card"],
        Mode = "payment",
        SuccessUrl = _successUrl,
        CancelUrl = _cancelUrl,
        ExpiresAt = DateTime.UtcNow.AddMinutes(30),
        LineItems =
        [
          new SessionLineItemOptions
        {
          PriceData = new SessionLineItemPriceDataOptions
          {
            Currency = "eur",
            UnitAmount = booking.Price * 100,
            ProductData = new SessionLineItemPriceDataProductDataOptions
            {
              Name = $"Your Booking {createStripeSessionDto.RoomId}",
              Description = $"{createStripeSessionDto.NumberOfDays} days booking"
            }
          },
          Quantity = 1
        }
        ],
        Metadata = new Dictionary<string, string>
      {
        { "BookingId", booking.Id.ToString() },
        { "RoomId", createStripeSessionDto.RoomId },
        { "UserId", createStripeSessionDto.UserId.ToString() },
        { "MoreThanTwoPets", createStripeSessionDto.MoreThanTwoPets.ToString() },
        { "WholeHouse", createStripeSessionDto.WholeHouse.ToString() },
        { "Price", createStripeSessionDto.Price.ToString() },
        { "NumberOfDays", createStripeSessionDto.NumberOfDays.ToString() },
        { "startDate", createStripeSessionDto.StartDate.ToString("o") },
        { "endDate", createStripeSessionDto.EndDate.ToString("o") }
      }
      };

      var service = new SessionService();
      var session = await service.CreateAsync(options);
      try
      {
        await _bookingDomainService.UpdateBookingStripeIdAsync(booking.Id, session.Id);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error updating booking with Stripe ID: {ex.Message}");
      }

      return session.Id;
    }
    catch (StripeException ex)
    {
      Console.WriteLine($"Stripe error: {ex.Message}");
      throw new Exception("Failed to create Stripe session. Please try again later.");
    }
  }


  public async Task HandleStripeWebhookAsync(string json, HttpRequest request)
  {
    var stripeEvent = EventUtility.ConstructEvent(
            json,
            request.Headers["Stripe-Signature"],
            _webhookSecret);

    if (stripeEvent.Type == "checkout.session.completed")
    {
      var session = stripeEvent.Data.Object as Session;

      if (session != null && session.Metadata.TryGetValue("BookingId", out var bookingIdStr)
          && Guid.TryParse(bookingIdStr, out var bookingId))
      {
        try
        {
          Booking booking = await _bookingDomainService.BookAsync(bookingId);

          try
          {
            User user = await _authService.GetUserByIdAsync(booking.UserId) ?? throw new Exception("User not found");
            _ = _emailService.SendSuccessPaymentEmailAsync(user.Email ?? throw new Exception("User don't have email"), new Booking
            (
              booking.Id,
              booking.Price,
              booking.NumberOfDays,
              booking.RoomId,
              booking.UserId,
              booking.Status,
              booking.StartDate,
              booking.EndDate,
              booking.MoreThanTwoPets,
              booking.ExpiresAt,
              booking.WholeHouse
            ));
          }
          catch (Exception ex)
          {
            Console.WriteLine($"Error retrieving user: {ex.Message}");
          }
        }
        catch
        {
          try
          {
            var s = session ?? throw new InvalidOperationException("Session is null");
            if (session.Metadata.TryGetValue("UserId", out var userIdStr)
            && Guid.TryParse(userIdStr, out var userId))
            {
              User user = await _authService.GetUserByIdAsync(userId) ?? throw new Exception("User not found");
              _ = _emailService.SendFailedPaymentEmailAsync(user.Email ?? throw new Exception("User don't have email"));
            }

          }
          catch
          {
            Console.WriteLine("Error retrieving user for failed payment email.");
          }
          var refundOptions = new RefundCreateOptions
          {
            PaymentIntent = session.PaymentIntentId,
            Reason = RefundReasons.Duplicate
          };
          var refundService = new RefundService();

          await refundService.CreateAsync(refundOptions);
        }
      }
    }
  }

  public async Task<List<BookingDto>> GetBookingsAsync()
  {
    return await _bookingDomainService.GetBookingsAsync();
  }

  public async Task BookByAdminAsync(CreateAdminBookingDto createAdmin)
  {
    await _bookingDomainService.CreateAdminBookingAsync(new Booking
    (
      1,
      1,
      createAdmin.RoomId,
      createAdmin.UserId,
      BookingStatus.ClosedByAdmin,
      createAdmin.StartDate,
      createAdmin.EndDate,
      false,
      null,
      createAdmin.WholeHouse
    ));
  }

  public async Task DeleteBookingByIdAsync(Guid bookingId)
  {
    await _bookingDomainService.DeleteBookingByIdAsync(bookingId);
  }
}