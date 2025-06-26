using Hengeler.Domain.Entities;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Domain.Services;

public class SlideDomainService(AppDbContext context) : ISlideDomainService
{
  private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

  public async Task<IEnumerable<Slide>> GetAllSlidesAsync()
  {
    return await _context.Slides
      .AsNoTracking()
      .ToListAsync();
  }

  public async Task<IEnumerable<Slide>> GetSlidesByIdsAsync(IEnumerable<Guid> ids)
  {
    return await _context.Slides
      .AsNoTracking()
      .Where(s => ids.Contains(s.Id))
      .ToListAsync();
  }

  public async Task<Slide> CreateSlideAsync(Slide slide)
  {
    ArgumentNullException.ThrowIfNull(slide);

    slide.Id = Guid.NewGuid();

    await _context.Slides.AddAsync(slide);
    await _context.SaveChangesAsync();

    return slide;
  }

  public async Task UpdateSlideAsync(Slide slide)
  {
    ArgumentNullException.ThrowIfNull(slide);

    var existingSlide = await _context.Slides
      .AsTracking()
      .FirstOrDefaultAsync(s => s.Id == slide.Id) ?? throw new InvalidOperationException($"Slide with id '{slide.Id}' not found.");
    existingSlide.ImageUrl = slide.ImageUrl;
    existingSlide.TitleKey = slide.TitleKey;
    existingSlide.DescriptionKey = slide.DescriptionKey;
    existingSlide.Price = slide.Price;

    await _context.SaveChangesAsync();
  }

  public async Task DeleteSlideAsync(Guid slideId)
  {
    var existingSlide = await _context.Slides
      .AsTracking()
      .FirstOrDefaultAsync(s => s.Id == slideId);

    if (existingSlide == null)
      throw new InvalidOperationException($"Slide with id '{slideId}' not found.");

    var translationKeys = new[] { existingSlide.TitleKey, existingSlide.DescriptionKey };

    var translationsToDelete = await _context.Translations
      .Where(t => translationKeys.Contains(t.Key))
      .ToListAsync();

    _context.Translations.RemoveRange(translationsToDelete);
    _context.Slides.Remove(existingSlide);

    await _context.SaveChangesAsync();
  }
}
