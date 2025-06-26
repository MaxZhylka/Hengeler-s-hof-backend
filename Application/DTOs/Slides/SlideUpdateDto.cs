namespace Hengeler.Application.DTOs.Slides;

public class SlideUpdateDto
{
  public Guid Id { get; set; }
  public Guid? TitleKey { get; set; }
  public Guid? DescriptionKey { get; set; }
  public string? UkTitle { get; set; } = default!;
  public string? EnTitle { get; set; } = default!;
  public string? DeTitle { get; set; } = default!;
  public string? UkDescription { get; set; } = default!;
  public string? EnDescription { get; set; } = default!;
  public string? DeDescription { get; set; } = default!;
  public string? Price { get; set; }
}
