using Hengeler.Application.DTOs.Event;
using Hengeler.Domain.Entities;

namespace Hengeler.Application.Interfaces;

public interface IEventService
{
    Task<Guid> CreateEventAsync(EventCreateDto dto, IFormFile? imageFile, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default);
    Task UpdateEventAsync(EventUpdateDto dto, IFormFile? newImageFile, CancellationToken cancellationToken = default);

    Task<IEnumerable<Event>> GetAllActiveEventsAsync(CancellationToken cancellationToken);
    Task SetEventActiveStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken);

    Task DeleteEventByIdAsync(Guid id, CancellationToken cancellationToken);
}
