using System.ComponentModel.DataAnnotations;

namespace Hengeler.Application.DTOs.Event;

public class EventUpdateFormModel
{
  [Required]
  public string Id { get; set; } = string.Empty;

  [Required]
  public string TitleKey { get; set; } = string.Empty;

  [Required]
  public string DescriptionKey { get; set; } = string.Empty;

  [Required]
  public string UkTitle { get; set; } = string.Empty;

  [Required]
  public string EnTitle { get; set; } = string.Empty;

  [Required]
  public string DeTitle { get; set; } = string.Empty;

  [Required]
  public string UkDescription { get; set; } = string.Empty;

  [Required]
  public string EnDescription { get; set; } = string.Empty;

  [Required]
  public string DeDescription { get; set; } = string.Empty;

  public string ImageUrl { get; set; } = string.Empty;

  public string Link { get; set; } = string.Empty;

  public bool OneDayEvent { get; set; }

  public bool IsActive { get; set; }

  [Required]
  public string StartDate { get; set; } = string.Empty;

  [Required]
  public string EndDate { get; set; } = string.Empty;

  [Required]
  public string StartTime { get; set; } = string.Empty;

  public IFormFile? Image { get; set; }
}
