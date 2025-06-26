using Hengeler.Application.DTOs.Room;
using Hengeler.Application.Interfaces;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Entities.Interfaces;
using Hengeler.Domain.Interfaces;

namespace Hengeler.Application.Services;
public class RoomService(
  IRoomDomainService roomService,
  ITranslationDomainService translationService
) : IRoomService
{
  private readonly IRoomDomainService _roomService = roomService;
  private readonly ITranslationDomainService _translationService = translationService;

  public async Task<RoomDto> GetRoomByIdAsync(string id)
  {
    var room = await _roomService.GetRoomById(id);
    return MapToDto(room);
  }

  public async Task<RoomDto> CreateOrUpdateRoomAsync(CreateOrUpdateRoomDto dto)
  {
    bool isCreate = dto.Id == Guid.Empty;

    var nameKey = isCreate ? Guid.NewGuid() : (await _roomService.GetRoomById(dto.RoomId)).NameKey;
    var descriptionKey = isCreate ? Guid.NewGuid() : (await _roomService.GetRoomById(dto.RoomId)).DescriptionKey;
    var maxGuestsKey = isCreate ? Guid.NewGuid() : (await _roomService.GetRoomById(dto.RoomId)).MaxGuestsKey;

    var translations = new List<Translations>
    {
      new() { Key = nameKey, Uk = dto.NameUk, En = dto.NameEn, De = dto.NameDe },
      new() { Key = descriptionKey, Uk = dto.DescriptionUk, En = dto.DescriptionEn, De = dto.DescriptionDe },
      new() { Key = maxGuestsKey, Uk = dto.MaxGuestsUk, En = dto.MaxGuestsEn, De = dto.MaxGuestsDe }
    };

    if (isCreate)
      await _translationService.CreateTranslationsAsync(translations);
    else
      await _translationService.UpdateTranslationsAsync(translations);

    var room = new Room
    {
      Id = dto.Id,
      RoomId = dto.RoomId,
      NameKey = nameKey,
      DescriptionKey = descriptionKey,
      MaxGuestsKey = maxGuestsKey,
      Price = dto.Price,
      AdditionalPrice = dto.AdditionalPrice,
      CheckIn = dto.CheckIn,
      CheckOut = dto.CheckOut,
      Size = dto.Size,
      SlideIds = dto.SlideIds
    };

    var saved = await _roomService.CreateOrUpdateRoomAsync(room);
    return MapToDto(saved);
  }

  private static RoomDto MapToDto(Room room) => new()
  {
    Id = room.Id,
    RoomId = room.RoomId,
    NameKey = room.NameKey,
    DescriptionKey = room.DescriptionKey,
    Price = room.Price,
    AdditionalPrice = room.AdditionalPrice,
    CheckIn = room.CheckIn,
    CheckOut = room.CheckOut,
    MaxGuestsKey = room.MaxGuestsKey,
    Size = room.Size,
    Slides = room.Slides.Select(s => new Slide
    {
      Id = s.Id,
      ImageUrl = s.ImageUrl,
      TitleKey = s.TitleKey,
      DescriptionKey = s.DescriptionKey,
      Price = s.Price
    }).ToList()
  };
}
