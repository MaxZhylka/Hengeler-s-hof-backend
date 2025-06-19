
using System.Text.Json.Serialization;

namespace Hengeler.Application.DTOs.Booking;

public class BookingDto
{
    public Guid Id { get; set; }
    public string RoomId { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookingStatus Status { get; set; }
    public string? CustomerEmail { get; set; }
    public string? StripeId { get; set; }
    public Guid UserId { get; set; }
    public string? CustomerPhone { get; set; }
    public required string CustomerName { get; set; }
    public bool MoreThanTwoPets { get; set; }
}