using System.ComponentModel.DataAnnotations;

namespace Hengeler.Domain.Entities;

public class Translations {
  [Key]
  public Guid Id { get; set; } = Guid.NewGuid();

  public Guid Key { get; set; }

  public string Uk { get; set; } = string.Empty;

  public string De { get; set; } = string.Empty;

  public string En { get; set; } = string.Empty;
}