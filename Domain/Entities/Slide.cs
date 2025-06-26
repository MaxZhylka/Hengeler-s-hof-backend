
using System.ComponentModel.DataAnnotations;

namespace Hengeler.Domain.Entities;

public class Slide
{
  [Key]
  public Guid Id { get; set; }
  public required string ImageUrl { get; set; }
  public Guid TitleKey { get; set; }
  public Guid DescriptionKey { get; set; }
  public string? Price { get; set; }
}