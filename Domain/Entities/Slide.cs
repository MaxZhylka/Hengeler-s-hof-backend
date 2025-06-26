
using System.ComponentModel.DataAnnotations;

namespace Hengeler.Domain.Entities;

public class Slide
{
  [Key]
  public Guid Id { get; set; }
  public required string ImageUrl { get; set; }
  public Guid? TitleKey { get; set; } = null;
  public Guid? DescriptionKey { get; set; } = null;
  public string? Price { get; set; }
}