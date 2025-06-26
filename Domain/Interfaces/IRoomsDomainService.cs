
namespace Hengeler.Domain.Entities.Interfaces;
public interface IRoomDomainService
{
  Task<Room> GetRoomById(string id);
  Task<Room> CreateOrUpdateRoomAsync(Room room);
}
