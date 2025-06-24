using System.ComponentModel.DataAnnotations;

namespace Hengeler.Application.DTOs.Event;
public class EventCreateDto
{

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

    public bool IsActive { get; set; } = true;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }
}
