using System.ComponentModel.DataAnnotations;

namespace Hengeler.Domain.Entities;

public class Feature
{
  [Key]
  public Guid Id { get; set; } = Guid.NewGuid();

  public required string FeatureName { get; set; }

  public bool IsActive { get; set; }
}