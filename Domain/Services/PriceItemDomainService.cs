using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Domain.Services;

public class PriceItemDomainService(AppDbContext context) : IPriceItemDomainService
{
  private readonly AppDbContext _context = context;

  public async Task<List<PriceItem>> GetAllAsync()
  {
    return await _context.PriceItems
      .OrderBy(p => p.NumOfPersons)
      .ToListAsync();
  }

  public async Task<List<PriceItem>> ReplaceAllAsync(List<PriceItem> incomingItems)
  {
    var existingItems = await _context.PriceItems.AsTracking().ToListAsync();

    foreach (var item in incomingItems)
    {
      var existing = existingItems.FirstOrDefault(x => x.Id == item.Id);
      if (existing != null)
      {
        existing.NumOfPersons = item.NumOfPersons;
        existing.Price = item.Price;
        _context.PriceItems.Update(existing);
      }
      else
      {
        _context.PriceItems.Add(item);
      }
    }

    var incomingIds = incomingItems.Select(i => i.Id).ToHashSet();
    var toRemove = existingItems.Where(x => !incomingIds.Contains(x.Id)).ToList();

    if (toRemove.Count > 0)
    {
      _context.PriceItems.RemoveRange(toRemove);
    }

    await _context.SaveChangesAsync();

    return await GetAllAsync();
  }
}
