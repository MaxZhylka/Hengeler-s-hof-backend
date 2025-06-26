
using Hengeler.Domain.Entities;

namespace Hengeler.Domain.Interfaces;
public interface IRoomDomainService
{
  Task<Room> GetRoomById(string id);
  Task<Room> CreateOrUpdateRoomAsync(Room room);
}
