using Hengeler.Domain.Entities;

namespace Hengeler.Domain.Interfaces;

public interface IPriceItemDomainService
{
  Task<List<PriceItem>> GetAllAsync();
  Task<List<PriceItem>> ReplaceAllAsync(List<PriceItem> items);
}
