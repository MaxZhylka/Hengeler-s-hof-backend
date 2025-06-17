
using Hengeler.Application.DTOs.Booking;
using Hengeler.Application.Interfaces;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace Hengeler.Application.Services;

public class BookingService(string stripeApiKey, string successUrl, string cancelUrl, string WebhookSecret, IEmailDomainService emailService, IAuthDomainService authService, IBookingDomainService bookingDomainService) : IBookingService
{
  private readonly string _stripeApiKey = stripeApiKey;
  private readonly string _successUrl = successUrl;
  private readonly string _cancelUrl = cancelUrl;
  private readonly string _webhookSecret = WebhookSecret;
  private readonly IAuthDomainService _authService = authService;
  private readonly IEmailDomainService _emailService = emailService;

  private readonly IBookingDomainService _bookingDomainService = bookingDomainService;
  public async Task<string> CreateStripeSessionAsync(CreateStripeSessionDto createStripeSessionDto)
  {

    var booking = await _bookingDomainService.CreatePendingBookingAsync(createStripeSessionDto);

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
            UnitAmount = createStripeSessionDto.Price * 100,
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
        { "MoreThanTwoPats", createStripeSessionDto.MoreThanTwoPats.ToString() },
        { "WholeHouse", createStripeSessionDto.WholeHouse.ToString() },
        { "Price", createStripeSessionDto.Price.ToString() },
        { "NumberOfDays", createStripeSessionDto.NumberOfDays.ToString() },
        { "startDate", createStripeSessionDto.StartDate.ToString("o") },
        { "endDate", createStripeSessionDto.EndDate.ToString("o") }
      }
    };

    var service = new SessionService();
    var session = await service.CreateAsync(options);
    return session.Id;
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
            _ = _emailService.SendSuccessPaymentEmailAsync(user.Email, new CreateStripeSessionDto
            {
              Price = booking.Price,
              NumberOfDays = booking.NumberOfDays,
              RoomId = booking.RoomId,
              UserId = booking.UserId,
              StartDate = booking.StartDate,
              EndDate = booking.EndDate,
              MoreThanTwoPats = booking.MoreThanTwoPats,
              WholeHouse = booking.WholeHouse
            });
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
            if (session != null && session.Metadata.TryGetValue("UserId", out var userIdStr)
            && Guid.TryParse(userIdStr, out var userId))
            {
              User user = await _authService.GetUserByIdAsync(userId) ?? throw new Exception("User not found");
              _ = _emailService.SendFailedPaymentEmailAsync(user.Email);
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
    return await _bookingDomainService.GetBookings();
  }
}