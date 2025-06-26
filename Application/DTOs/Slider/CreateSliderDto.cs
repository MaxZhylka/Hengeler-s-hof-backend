namespace Hengeler.Application.DTOs.Slider;
public class CreateSliderDto
{
  public Guid Id { get; set; }
  public string SliderId { get; set; } = string.Empty;
  public List<Guid> SlideIds { get; set; } = [];
}