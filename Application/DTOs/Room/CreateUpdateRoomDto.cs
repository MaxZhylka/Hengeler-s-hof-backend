
namespace Hengeler.Application.DTOs.Room;
public class CreateOrUpdateRoomDto
{
  public Guid Id { get; set; }
  public required string RoomId { get; set; }

  public Guid? NameKey { get; set; }
  public Guid? DescriptionKey { get; set; }
  public Guid? MaxGuestsKey { get; set; }

  public string NameUk { get; set; } = "";
  public string NameEn { get; set; } = "";
  public string NameDe { get; set; } = "";

  public string DescriptionUk { get; set; } = "";
  public string DescriptionEn { get; set; } = "";
  public string DescriptionDe { get; set; } = "";

  public string MaxGuestsUk { get; set; } = "";
  public string MaxGuestsEn { get; set; } = "";
  public string MaxGuestsDe { get; set; } = "";

  public int Price { get; set; }
  public int AdditionalPrice { get; set; }
  public TimeOnly CheckIn { get; set; }
  public TimeOnly CheckOut { get; set; }
  public int Size { get; set; }

  public List<Guid> SlideIds { get; set; } = [];
}
