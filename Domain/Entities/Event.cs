using System.ComponentModel.DataAnnotations;

namespace Hengeler.Domain.Entities;
public class Event {
  [Key]
  public Guid Id { get; set; } = Guid.NewGuid();

  public Guid TitleKey { get; set; }

  public Guid DescriptionKey { get; set; }

  public string ImageUrl { get; set; } = string.Empty;

  public string Link { get; set; } = string.Empty;

  public bool OneDayEvent { get; set; }

  public bool IsActive { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateOnly StartDate { get; set; }

  public DateOnly EndDate { get; set; }

  public TimeOnly StartTime { get; set; }

}