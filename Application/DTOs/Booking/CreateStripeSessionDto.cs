using System.ComponentModel.DataAnnotations;

namespace Hengeler.Application.DTOs.Booking;

public class CreateStripeSessionDto
{
  [Required]
  [Range(1, int.MaxValue, ErrorMessage = "Price must be a positive number.")]
  public int Price { get; set; }
  public int NumberOfDays { get; set; }

  [Required]
  public required string RoomId { get; set; }
  [Required]
  public Guid UserId { get; set; }
  public DateOnly StartDate { get; set; }
  public DateOnly EndDate { get; set; }

  public bool MoreThanTwoPets { get; set; }

  public bool WholeHouse { get; set; } = false;

}