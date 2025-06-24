using Hengeler.Domain.Entities;

namespace Hengeler.Domain.Interfaces;

public interface IEventDomainService
{
    Task<Event> CreateEventAsync(Event newEvent, CancellationToken cancellationToken = default);
    Task<Event?> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default);
    Task UpdateEventAsync(Event updatedEvent, CancellationToken cancellationToken = default);
    Task DeleteEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task SetEventActiveStatusAsync(Guid eventId, bool isActive, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetAllActiveEventsAsync(CancellationToken cancellationToken = default);
}
