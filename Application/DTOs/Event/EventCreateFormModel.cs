namespace Hengeler.Application.DTOs.Event;
public class EventCreateFormModel
{
  public string UkTitle { get; set; } = string.Empty;
  public string EnTitle { get; set; } = string.Empty;
  public string DeTitle { get; set; } = string.Empty;

  public string UkDescription { get; set; } = string.Empty;
  public string EnDescription { get; set; } = string.Empty;
  public string DeDescription { get; set; } = string.Empty;

  public string? Link { get; set; }

  public bool OneDayEvent { get; set; }
  public bool IsActive { get; set; }

  public string StartDate { get; set; } = string.Empty; // parse to DateOnly
  public string EndDate { get; set; } = string.Empty;
  public string StartTime { get; set; } = string.Empty;

  public IFormFile? Image { get; set; }
}
