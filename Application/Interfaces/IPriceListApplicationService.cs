
using Hengeler.Application.DTOs.PriceList;

namespace Hengeler.Application.Interfaces;

public interface IPriceListApplicationService
{
  Task<List<PriceItemDto>> GetAllAsync();
  Task<List<PriceItemDto>> ReplaceAllAsync(List<PriceItemDto> items);
}
