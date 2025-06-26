


using System.ComponentModel.DataAnnotations;

namespace Hengeler.Application.DTOs.Booking;

public class CreateAdminBookingDto
{
  [Required]
  public string RoomId { get; set; }
  [Required]
  public Guid UserId { get; set; }
  public DateOnly StartDate { get; set; }
  public DateOnly EndDate { get; set; }

  public bool WholeHouse { get; set; }
}
