
namespace Hengeler.Application.DTOs.Booking;

public class BookingDto
{
    public Guid Id { get; set; }
    public string RoomId { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "";
    public string? StripeSessionId { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? CustomerEmail { get; set; }
}