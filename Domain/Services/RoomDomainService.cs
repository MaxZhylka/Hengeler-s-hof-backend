using Hengeler.Domain.Entities;
using Hengeler.Domain.Interfaces;
using Hengeler.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Hengeler.Domain.Services;

public class RoomDomainService(AppDbContext context) : IRoomDomainService
{
  private readonly AppDbContext _context = context;

  public async Task<Room> GetRoomById(string id)
  {

    var room = await _context.Rooms
        .FirstOrDefaultAsync(r => r.RoomId == id)
        ?? throw new KeyNotFoundException("There is no room with such id");

    if (room.SlideIds != null && room.SlideIds.Count > 0)
    {
      var slides = await _context.Slides
            .Where(s => room.SlideIds.Contains(s.Id))
            .ToListAsync();

      var slidesById = slides.ToDictionary(s => s.Id);
      room.Slides = [.. room.SlideIds.Select(id => slidesById[id])];
    }
    else
    {
      room.Slides = [];
    }

    return room;
  }
  public async Task<Room> CreateOrUpdateRoomAsync(Room room)
  {
    var existing = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == room.Id);

    if (existing == null)
    {
      _context.Rooms.Add(room);
    }
    else
    {
      existing.NameKey = room.NameKey;
      existing.DescriptionKey = room.DescriptionKey;
      existing.Price = room.Price;
      existing.AdditionalPrice = room.AdditionalPrice;
      existing.CheckIn = room.CheckIn;
      existing.CheckOut = room.CheckOut;
      existing.MaxGuestsKey = room.MaxGuestsKey;
      existing.Size = room.Size;

      existing.SlideIds = room.SlideIds;

      _context.Rooms.Update(existing);
    }

    await _context.SaveChangesAsync();
    return room;
  }
}
