using Hengeler.Domain.Entities;

namespace Hengeler.Application.DTOs.Slider;
public class SliderDto
{
  public Guid Id { get; set; }
  public string SliderId { get; set; } = string.Empty;
  public List<Slide> Slides { get; set; } = [];
}