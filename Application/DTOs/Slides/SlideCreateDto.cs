namespace Hengeler.Application.DTOs.Slides;

public class SlideCreateDto
{
  public string UkTitle { get; set; } = default!;
  public string EnTitle { get; set; } = default!;
  public string DeTitle { get; set; } = default!;
  public string UkDescription { get; set; } = default!;
  public string EnDescription { get; set; } = default!;
  public string DeDescription { get; set; } = default!;
  public string? Price { get; set; }
}
