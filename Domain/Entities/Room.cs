using System.ComponentModel.DataAnnotations;

namespace Hengeler.Domain.Entities;

public class Room
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string RoomId { get; set; }

  public Guid NameKey { get; set; }

  public Guid DescriptionKey { get; set; }

  public int Price { get; set; }

  public int AdditionalPrice { get; set; }

  public TimeOnly CheckIn { get; set; }

  public TimeOnly CheckOut { get; set; }

  public Guid MaxGuestsKey { get; set; }

  public int Size { get; set; }

  public List<Guid> SlideIds { get; set; } = [];
  public ICollection<Slide> Slides { get; set; } = [];
}