using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Domain.Services;

public class SliderDomainService(AppDbContext context) : ISliderDomainService
{
  private readonly AppDbContext _context = context;

  public async Task<Slider> GetSliderByIdAsync(string id)
  {
    var slider = await _context.Sliders
        .FirstOrDefaultAsync(s => s.SliderId == id)
        ?? throw new KeyNotFoundException("There is no slider with such id");

    if (slider.SlideIds != null && slider.SlideIds.Count > 0)
    {
      var slides = await _context.Slides
          .Where(s => slider.SlideIds.Contains(s.Id))
          .ToListAsync();

      var slidesById = slides.ToDictionary(s => s.Id);

      var existingSlideIds = slider.SlideIds.Where(id => slidesById.ContainsKey(id)).ToList();

      if (existingSlideIds.Count != slider.SlideIds.Count)
      {
        slider.SlideIds = existingSlideIds;
        _context.Sliders.Update(slider);
        await _context.SaveChangesAsync();
      }

      slider.Slides = existingSlideIds.Select(id => slidesById[id]).ToList();
    }
    else
    {
      slider.Slides = new List<Slide>();
    }

    return slider;
  }

  public async Task<Slider> CreateOrUpdateSliderAsync(Slider slider)
  {
    var existing = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == slider.Id);

    if (existing == null)
    {
      _context.Sliders.Add(slider);
    }
    else
    {
      existing.SliderId = slider.SliderId;
      existing.SlideIds = slider.SlideIds;
      _context.Sliders.Update(existing);
    }

    await _context.SaveChangesAsync();
    return slider;
  }
}
