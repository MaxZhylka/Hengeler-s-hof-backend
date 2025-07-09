
using Hengeler.Application.DTOs.PriceList;
using Hengeler.Application.Interfaces;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;

namespace Hengeler.Application.Services;

public class PriceListApplicationService(IPriceItemDomainService domainService) : IPriceListApplicationService
{
  private readonly IPriceItemDomainService _domainService = domainService;

  public async Task<List<PriceItemDto>> GetAllAsync()
  {
    var items = await _domainService.GetAllAsync();
    return [.. items.Select(MapToDto)];
  }

  public async Task<List<PriceItemDto>> ReplaceAllAsync(List<PriceItemDto> dtos)
  {
    var entities = dtos.Select(MapToEntity).ToList();
    var updated = await _domainService.ReplaceAllAsync(entities);
    return [.. updated.Select(MapToDto)];
  }

  private static PriceItemDto MapToDto(PriceItem item)
  {
    return new PriceItemDto
    {
      Id = item.Id,
      NumOfPersons = item.NumOfPersons,
      Price = item.Price
    };
  }

  private static PriceItem MapToEntity(PriceItemDto dto)
  {
    return new PriceItem
    {
      Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
      NumOfPersons = dto.NumOfPersons,
      Price = dto.Price
    };
  }
}
