
using System.Text.Json.Serialization;

namespace Hengeler.Application.DTOs.Booking;

public class BookingDto
{
    public Guid Id { get; set; }
    public string RoomId { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookingStatus Status { get; set; }
    public string? CustomerEmail { get; set; }

    public string? CustomerPhone { get; set; }
    public required string CustomerName { get; set; }
    public bool MoreThanTwoPets { get; set; }
}