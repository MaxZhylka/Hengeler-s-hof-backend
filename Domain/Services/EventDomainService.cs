using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Domain.Services;

public class EventDomainService(AppDbContext context) : IEventDomainService
{
    private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<Event> CreateEventAsync(Event newEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(newEvent);

        newEvent.Id = Guid.NewGuid();
        newEvent.CreatedAt = DateTime.UtcNow;

        await _context.Events.AddAsync(newEvent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return newEvent;
    }

    public async Task<Event?> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetAllActiveEventsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Events.Where(e => e.IsActive)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateEventAsync(Event updatedEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updatedEvent);

        var existingEvent = await _context.Events.AsTracking()
            .FirstOrDefaultAsync(e => e.Id == updatedEvent.Id, cancellationToken);

        if (existingEvent == null)
            throw new InvalidOperationException($"Event with id '{updatedEvent.Id}' not found.");

        existingEvent.TitleKey = updatedEvent.TitleKey;
        existingEvent.DescriptionKey = updatedEvent.DescriptionKey;
        existingEvent.ImageUrl = updatedEvent.ImageUrl;
        existingEvent.Link = updatedEvent.Link;
        existingEvent.OneDayEvent = updatedEvent.OneDayEvent;
        existingEvent.IsActive = updatedEvent.IsActive;
        existingEvent.StartDate = updatedEvent.StartDate;
        existingEvent.EndDate = updatedEvent.EndDate;
        existingEvent.StartTime = updatedEvent.StartTime;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var existingEvent = await _context.Events
            .AsTracking()
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken) ?? throw new InvalidOperationException($"Event with id '{eventId}' not found.");
        var translationKeys = new[] { existingEvent.TitleKey, existingEvent.DescriptionKey };

        var translationsToDelete = await _context.Translations
            .Where(t => translationKeys.Contains(t.Key))
            .ToListAsync(cancellationToken);

        _context.Translations.RemoveRange(translationsToDelete);

        _context.Events.Remove(existingEvent);
        await _context.SaveChangesAsync(cancellationToken);
    }


    public async Task SetEventActiveStatusAsync(Guid eventId, bool isActive, CancellationToken cancellationToken = default)
    {
        var existingEvent = await _context.Events.AsTracking()
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken) ?? throw new InvalidOperationException($"Event with id '{eventId}' not found.");
        existingEvent.IsActive = isActive;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
