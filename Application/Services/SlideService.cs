using Hengeler.Application.DTOs.Slides;
using Hengeler.Application.Interfaces;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Domain.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Processing;

namespace Hengeler.Application.Services;

public class SlideService(
  ISlideDomainService slideService,
  ITranslationDomainService translationService,
    IConfiguration config
) : ISlideService
{
  private readonly ISlideDomainService _slideService = slideService;
  private readonly ITranslationDomainService _translationService = translationService;
  private readonly string _storagePath = config["FileStorage:BasePath"]
      ?? throw new InvalidOperationException("File storage path is not configured");

  public async Task<Guid> CreateSlideAsync(SlideCreateDto dto, IFormFile? imageFile, CancellationToken cancellationToken = default)
  {
    var imageUrl = imageFile is not null
      ? await SaveImageAsync(imageFile, cancellationToken)
      : string.Empty;

    var titleKey = Guid.NewGuid();
    var descKey = Guid.NewGuid();

    var translations = new List<Translations>();

    bool hasAnyTitle = !string.IsNullOrWhiteSpace(dto.UkTitle)
                    || !string.IsNullOrWhiteSpace(dto.EnTitle)
                    || !string.IsNullOrWhiteSpace(dto.DeTitle);

    bool hasAllTitle = !string.IsNullOrWhiteSpace(dto.UkTitle)
                    && !string.IsNullOrWhiteSpace(dto.EnTitle)
                    && !string.IsNullOrWhiteSpace(dto.DeTitle);

    if (hasAllTitle)
    {
      translations.Add(new Translations
      {
        Key = titleKey,
        Uk = dto.UkTitle!,
        En = dto.EnTitle!,
        De = dto.DeTitle!
      });
    }
    else if (hasAnyTitle)
    {
      throw new ArgumentException("If one of the title translations is filled, all must be filled.");
    }

    bool hasAnyDesc = !string.IsNullOrWhiteSpace(dto.UkDescription)
                    || !string.IsNullOrWhiteSpace(dto.EnDescription)
                    || !string.IsNullOrWhiteSpace(dto.DeDescription);

    bool hasAllDesc = !string.IsNullOrWhiteSpace(dto.UkDescription)
                    && !string.IsNullOrWhiteSpace(dto.EnDescription)
                    && !string.IsNullOrWhiteSpace(dto.DeDescription);

    if (hasAllDesc)
    {
      translations.Add(new Translations
      {
        Key = descKey,
        Uk = dto.UkDescription!,
        En = dto.EnDescription!,
        De = dto.DeDescription!
      });
    }
    else if (hasAnyDesc)
    {
      throw new ArgumentException("If one of the description translations is filled, all must be filled.");
    }

    if (translations.Count > 0)
    {
      await _translationService.CreateTranslationsAsync(translations);
    }

    var slide = new Slide
    {
      Id = Guid.NewGuid(),
      TitleKey = !hasAllTitle ? null : titleKey,
      DescriptionKey = !hasAllDesc ? null : descKey,
      ImageUrl = imageUrl,
      Price = dto.Price
    };

    var result = await _slideService.CreateSlideAsync(slide);
    return result.Id;
  }

  public async Task UpdateSlideAsync(SlideUpdateDto dto, IFormFile? newImageFile, CancellationToken cancellationToken = default)
  {
    var translations = new List<Translations>();

    bool hasAnyTitle = !string.IsNullOrWhiteSpace(dto.UkTitle)
                    || !string.IsNullOrWhiteSpace(dto.EnTitle)
                    || !string.IsNullOrWhiteSpace(dto.DeTitle);

    bool hasAllTitle = !string.IsNullOrWhiteSpace(dto.UkTitle)
                    && !string.IsNullOrWhiteSpace(dto.EnTitle)
                    && !string.IsNullOrWhiteSpace(dto.DeTitle);

    if (hasAllTitle)
    {
      translations.Add(new Translations
      {
        Key = dto.TitleKey ?? Guid.NewGuid(),
        Uk = dto.UkTitle!,
        En = dto.EnTitle!,
        De = dto.DeTitle!
      });
    }
    else if (hasAnyTitle)
    {
      throw new ArgumentException("If one of the title translation fields is filled, all must be provided.");
    }


    bool hasAnyDesc = !string.IsNullOrWhiteSpace(dto.UkDescription)
                    || !string.IsNullOrWhiteSpace(dto.EnDescription)
                    || !string.IsNullOrWhiteSpace(dto.DeDescription);

    bool hasAllDesc = !string.IsNullOrWhiteSpace(dto.UkDescription)
                    && !string.IsNullOrWhiteSpace(dto.EnDescription)
                    && !string.IsNullOrWhiteSpace(dto.DeDescription);


    if (hasAllDesc)
    {
      translations.Add(new Translations
      {
        Key = dto.DescriptionKey ?? Guid.NewGuid(),
        Uk = dto.UkDescription!,
        En = dto.EnDescription!,
        De = dto.DeDescription!
      });
    }
    else if (hasAnyDesc)
    {
      throw new ArgumentNullException("Translations not filled");
    }


    if (translations.Count != 0)
    {
      await _translationService.UpdateTranslationsAsync(translations);
    }

    var existingSlide = await _slideService.GetSlidesByIdsAsync(new[] { dto.Id })
      .ContinueWith(t => t.Result.FirstOrDefault(), cancellationToken);

    if (existingSlide is null)
      throw new InvalidOperationException("Slide not found");

    if (newImageFile is not null)
    {
      if (!string.IsNullOrEmpty(existingSlide.ImageUrl))
      {
        var relativePath = existingSlide.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

        var absolutePath = Path.Combine(_storagePath, relativePath);

        try
        {
          if (File.Exists(absolutePath))
          {
            File.Delete(absolutePath);
          }
          else
          {
            Console.WriteLine($"Warning: File '{absolutePath}' not found for deletion.");
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Error deleting file '{absolutePath}': {ex.Message}");
        }
      }
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
    var slides = await _slideService.GetSlidesByIdsAsync(new[] { id });
    var slide = slides.FirstOrDefault();

    if (slide == null)
      throw new InvalidOperationException("Slide not found");

    string? imageUrl = slide.ImageUrl;

    await _slideService.DeleteSlideAsync(id);

    if (!string.IsNullOrEmpty(imageUrl))
    {
      var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

      var absolutePath = Path.Combine(_storagePath, relativePath);

      try
      {
        if (File.Exists(absolutePath))
        {
          File.Delete(absolutePath);
        }
        else
        {
          Console.WriteLine($"Warning: File '{absolutePath}' not found for deletion.");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error deleting file '{absolutePath}': {ex.Message}");
      }
    }
  }
  private async Task<string> SaveImageAsync(IFormFile image, CancellationToken cancellationToken)
  {
    var imageName = $"{Guid.NewGuid()}.webp";
    var imagePath = Path.Combine(_storagePath, "images", "slides");

    if (!Directory.Exists(imagePath))
      Directory.CreateDirectory(imagePath);

    var fullPath = Path.Combine(imagePath, imageName);

    using var imageStream = image.OpenReadStream();
    using var img = await Image.LoadAsync(imageStream, cancellationToken);

    var exif = img.Metadata.ExifProfile;
    ushort? orientation = null;

    if (exif != null && exif.TryGetValue(ExifTag.Orientation, out IExifValue<ushort>? o))
    {
      orientation = o.Value;
    }

    switch (orientation)
    {
      case 2:
        img.Mutate(x => x.Flip(FlipMode.Horizontal));
        break;
      case 3:
        img.Mutate(x => x.Rotate(RotateMode.Rotate180));
        break;
      case 4:
        img.Mutate(x => x.Flip(FlipMode.Vertical));
        break;
      case 5:
        img.Mutate(x => { x.Flip(FlipMode.Vertical); x.Rotate(RotateMode.Rotate90); });
        break;
      case 6:
        img.Mutate(x => x.Rotate(RotateMode.Rotate90));
        break;
      case 7:
        img.Mutate(x => { x.Flip(FlipMode.Horizontal); x.Rotate(RotateMode.Rotate90); });
        break;
      case 8:
        img.Mutate(x => x.Rotate(RotateMode.Rotate270));
        break;
      default:
        break;
    }

    img.Metadata.ExifProfile = null;

    var encoder = new WebpEncoder()
    {
      Quality = 80
    };

    await img.SaveAsync(fullPath, encoder, cancellationToken);

    return $"/images/slides/{imageName}";
  }
}
