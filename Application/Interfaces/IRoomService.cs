using Hengeler.Application.DTOs.Room;

namespace Hengeler.Application.Interfaces;

public interface IRoomService
{
  Task<RoomDto> GetRoomByIdAsync(string id);
  Task<RoomDto> CreateOrUpdateRoomAsync(CreateOrUpdateRoomDto dto);
}
