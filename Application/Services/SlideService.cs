using Hengeler.Application.DTOs.Slides;
using Hengeler.Application.Interfaces;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Domain.Services;

namespace Hengeler.Application.Services;

public class SlideService(
  ISlideDomainService slideService,
  ITranslationDomainService translationService,
  IWebHostEnvironment environment
) : ISlideService
{
  private readonly ISlideDomainService _slideService = slideService;
  private readonly ITranslationDomainService _translationService = translationService;
  private readonly IWebHostEnvironment _environment = environment;

  public async Task<Guid> CreateSlideAsync(SlideCreateDto dto, IFormFile? imageFile, CancellationToken cancellationToken = default)
  {
    var imageUrl = imageFile is not null
      ? await SaveImageAsync(imageFile, cancellationToken)
      : string.Empty;

    var titleKey = Guid.NewGuid();
    var descKey = Guid.NewGuid();

    var translations = new List<Translations>
    {
      new() { Key = titleKey, Uk = dto.UkTitle, En = dto.EnTitle, De = dto.DeTitle },
      new() { Key = descKey, Uk = dto.UkDescription, En = dto.EnDescription, De = dto.DeDescription }
    };

    await _translationService.CreateTranslationsAsync(translations);

    var slide = new Slide
    {
      Id = Guid.NewGuid(),
      TitleKey = titleKey,
      DescriptionKey = descKey,
      ImageUrl = imageUrl,
      Price = dto.Price
    };

    var result = await _slideService.CreateSlideAsync(slide);
    return result.Id;
  }

  public async Task UpdateSlideAsync(SlideUpdateDto dto, IFormFile? newImageFile, CancellationToken cancellationToken = default)
  {
    var translations = new List<Translations>
    {
      new() { Key = dto.TitleKey, Uk = dto.UkTitle, En = dto.EnTitle, De = dto.DeTitle },
      new() { Key = dto.DescriptionKey, Uk = dto.UkDescription, En = dto.EnDescription, De = dto.DeDescription }
    };

    await _translationService.UpdateTranslationsAsync(translations);

    var existingSlide = await _slideService.GetSlidesByIdsAsync(new[] { dto.Id })
      .ContinueWith(t => t.Result.FirstOrDefault(), cancellationToken);

    if (existingSlide is null)
      throw new InvalidOperationException("Slide not found");

    if (newImageFile is not null)
    {
      existingSlide.ImageUrl = await SaveImageAsync(newImageFile, cancellationToken);
    }

    existingSlide.Price = dto.Price;

    await _slideService.UpdateSlideAsync(existingSlide);
  }

  public async Task<IEnumerable<Slide>> GetAllSlidesAsync(CancellationToken cancellationToken = default)
  {
    return await _slideService.GetAllSlidesAsync();
  }

  public async Task DeleteSlideAsync(Guid id, CancellationToken cancellationToken = default)
  {
    await _slideService.DeleteSlideAsync(id);
  }

  private async Task<string> SaveImageAsync(IFormFile image, CancellationToken cancellationToken)
  {
    var imageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
    var imagePath = Path.Combine(_environment.WebRootPath, "images", "slides");

    if (!Directory.Exists(imagePath))
      Directory.CreateDirectory(imagePath);

    var fullPath = Path.Combine(imagePath, imageName);

    await using var stream = new FileStream(fullPath, FileMode.Create);
    await image.CopyToAsync(stream, cancellationToken);

    return $"/images/slides/{imageName}";
  }
}
