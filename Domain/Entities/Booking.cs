using System.ComponentModel.DataAnnotations;

namespace Hengeler.Domain.Entities;

public class Booking
{
  [Key]
  public Guid Id { get; set; } = Guid.NewGuid();

  [Required]
  public int Price { get; set; }

  [Required]
  public int NumberOfDays { get; set; }

  [Required]
  public string RoomId { get; set; } = string.Empty;

  [Required]
  public Guid UserId { get; set; } = Guid.Empty;

  [Required]
  public DateTime StartDate { get; set; }

  [Required]
  public DateTime EndDate { get; set; }

  public bool MoreThanTwoPats { get; set; }

  public bool WholeHouse { get; set; }

  [Required]
  public BookingStatus Status { get; set; } = BookingStatus.Pending;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? ExpiresAt { get; set; }

  public User? User { get; set; } = null;

  protected Booking() { }

  public Booking(
    int price,
    int numberOfDays,
    string roomId,
    Guid userId,
    BookingStatus status,
    DateTime startDate,
    DateTime endDate,
    bool moreThanTwoPats,
    DateTime? expiresAt = null,
    bool wholeHouse = false
  )
  {
    Id = Guid.NewGuid();
    Price = price;
    NumberOfDays = numberOfDays;
    RoomId = roomId;
    UserId = userId;
    StartDate = startDate;
    EndDate = endDate;
    Status = status;
    MoreThanTwoPats = moreThanTwoPats;
    ExpiresAt = expiresAt;
    WholeHouse = wholeHouse;
  }
}
