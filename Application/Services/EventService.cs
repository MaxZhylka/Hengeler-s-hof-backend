using Hengeler.Application.DTOs.Event;
using Hengeler.Application.Interfaces;
using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;

namespace Hengeler.Application.Services;

public class EventService(
    IEventDomainService eventService,
    ITranslationDomainService translationService,
    IConfiguration config) : IEventService
{
    private readonly IEventDomainService _eventService = eventService;
    private readonly ITranslationDomainService _translationService = translationService;
    private readonly string _storagePath = config["FileStorage:BasePath"]
        ?? throw new InvalidOperationException("File storage path is not configured");

    public async Task<Guid> CreateEventAsync(EventCreateDto dto, IFormFile? imageFile, CancellationToken cancellationToken = default)
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

        var ev = new Event
        {
            TitleKey = titleKey,
            DescriptionKey = descKey,
            ImageUrl = imageUrl,
            Link = dto.Link,
            OneDayEvent = dto.OneDayEvent,
            IsActive = dto.IsActive,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            StartTime = dto.StartTime
        };

        var result = await _eventService.CreateEventAsync(ev, cancellationToken);
        return result.Id;
    }

    public async Task UpdateEventAsync(EventUpdateDto dto, IFormFile? newImageFile, CancellationToken cancellationToken = default)
    {
        var translations = new List<Translations>
        {
            new() { Key = dto.TitleKey, Uk = dto.UkTitle, En = dto.EnTitle, De = dto.DeTitle },
            new() { Key = dto.DescriptionKey, Uk = dto.UkDescription, En = dto.EnDescription, De = dto.DeDescription }
        };
        await _translationService.UpdateTranslationsAsync(translations);

        var existingEvent = await _eventService.GetEventByIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException("Event not found");

        if (newImageFile is not null)
        {
            if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                var relativePath = dto.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
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
            existingEvent.ImageUrl = await SaveImageAsync(newImageFile, cancellationToken);

        }

        existingEvent.Link = dto.Link;
        existingEvent.OneDayEvent = dto.OneDayEvent;
        existingEvent.IsActive = dto.IsActive;
        existingEvent.StartDate = dto.StartDate;
        existingEvent.EndDate = dto.EndDate;
        existingEvent.StartTime = dto.StartTime;

        await _eventService.UpdateEventAsync(existingEvent, cancellationToken);
    }

    public async Task SetEventActiveStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        await _eventService.SetEventActiveStatusAsync(id, isActive, cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetAllActiveEventsAsync(CancellationToken cancellationToken)
    {
        return await _eventService.GetAllActiveEventsAsync(cancellationToken);
    }

    public async Task DeleteEventByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var ev = await _eventService.GetEventByIdAsync(id, cancellationToken);
        if (ev == null)
            throw new InvalidOperationException("Event not found");

        string? imageUrl = ev.ImageUrl;

        await _eventService.DeleteEventAsync(id, cancellationToken);

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

    public async Task<IEnumerable<Event>> GetAllEventsAsync(CancellationToken cancellationToken)
    {
        return await _eventService.GetAllEventsAsync(cancellationToken);
    }
    private async Task<string> SaveImageAsync(IFormFile image, CancellationToken cancellationToken)
    {
        var imageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var imagePath = Path.Combine(_storagePath, "images", "events");

        if (!Directory.Exists(imagePath))
            Directory.CreateDirectory(imagePath);

        var fullPath = Path.Combine(imagePath, imageName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await image.CopyToAsync(stream, cancellationToken);

        return $"/images/events/{imageName}";
    }
}
