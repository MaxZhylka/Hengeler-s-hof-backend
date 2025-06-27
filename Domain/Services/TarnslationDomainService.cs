using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Domain.Services;

public class TranslationDomainService(AppDbContext context) : ITranslationDomainService
{
  private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

  public async Task<IEnumerable<Translations>> GetTranslationsAsync(IEnumerable<Guid> keys)
  {
    ArgumentNullException.ThrowIfNull(keys);

    return await _context.Translations
    .Where(t => keys.Contains(t.Key))
    .ToListAsync();
  }

  public async Task<IEnumerable<Translations>> GetAllTranslationsAsync()
  {
    return await _context.Translations.ToListAsync();
  }

  public async Task<Translations?> GetTranslationAsync(Guid key)
  {
    return await _context.Translations
        .FirstOrDefaultAsync(t => t.Key == key);
  }

  public async Task CreateTranslationsAsync(IEnumerable<Translations> translations)
  {
    ArgumentNullException.ThrowIfNull(translations);

    var keys = translations.Select(t => t.Key).ToList();

    var existingKeys = await _context.Translations
        .Where(t => keys.Contains(t.Key))
        .Select(t => t.Key)
        .ToListAsync();

    if (existingKeys.Count != 0)
    {
      throw new InvalidOperationException($"Translations with keys already exist: {string.Join(", ", existingKeys)}");
    }

    await _context.Translations.AddRangeAsync(translations);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateTranslationsAsync(IEnumerable<Translations> translations)
  {
      ArgumentNullException.ThrowIfNull(translations);

      var keys = translations.Select(t => t.Key).ToList();

      var existingTranslations = await _context.Translations.AsTracking()
          .Where(t => keys.Contains(t.Key))
          .ToListAsync();

      foreach (var updatedTranslation in translations)
      {
          var existing = existingTranslations.FirstOrDefault(t => t.Key == updatedTranslation.Key);

          if (existing == null)
          {
              await _context.Translations.AddAsync(updatedTranslation);
          }
          else
          {
              existing.Uk = updatedTranslation.Uk;
              existing.De = updatedTranslation.De;
              existing.En = updatedTranslation.En;
          }
      }

      await _context.SaveChangesAsync();
  }
}
