namespace Hengeler.Application.DTOs.PriceList;
public class PriceItemDto
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public int NumOfPersons { get; set; }
  public int Price { get; set; }
}