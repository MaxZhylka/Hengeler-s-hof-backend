using System.Text.Json;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Domain.Services;

public class FeatureService(AppDbContext context) : IFeatureService
{
  private readonly AppDbContext _context = context;

  public async Task<List<Feature>> GetAllAsync()
  {
    return await _context.Features.ToListAsync();
  }

  public async Task<bool> UpdateAsync(Feature dto)
  {
    var feature = await _context.Features.AsTracking().FirstOrDefaultAsync(f => f.Id == dto.Id);
    if (feature == null)
    {
      Console.WriteLine($"Feature с Id {dto.Id} не найден.");
      return false;
    }

    feature.IsActive = dto.IsActive;
    _context.Entry(feature).Property(f => f.IsActive).IsModified = true;

    var result = await _context.SaveChangesAsync();

    return true;
  }
}
