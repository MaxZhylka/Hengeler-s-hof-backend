using System.ComponentModel.DataAnnotations;

namespace Hengeler.Domain.Entities;

public class PriceItem
{
  [Key]
  public Guid Id { get; set; } = Guid.NewGuid();

  public int NumOfPersons { get; set; }

  public int Price { get; set; }
}