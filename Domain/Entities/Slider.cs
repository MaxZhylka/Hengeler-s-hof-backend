
using System.ComponentModel.DataAnnotations;

namespace Hengeler.Domain.Entities;

public class Slider
{
  [Key]
  public Guid Id { get; set; } = Guid.NewGuid();
  public string SliderId { get; set; }
  public List<Slide> Slides { get; set; }

  public List<Guid> SlideIds { get; set; }
}