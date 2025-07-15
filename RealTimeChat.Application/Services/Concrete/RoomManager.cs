using RealTimeChat.Application.DTOs.Room;
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Domain.Entities;
using RealTimeChat.Domain.Repositories;

namespace RealTimeChat.Application.Services.Concrete
{
    public class RoomManager : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomManager(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<Room> CreateRoomAsync(CreateRoomDto dto, Guid creatorUserId)
        {
            var roomId = Guid.NewGuid();

            var room = new Room
            {
                Id = roomId,
                Name = dto.Name,
                UserRooms = new List<UserRoom>
                {
                    new UserRoom
                    {
                        UserId = creatorUserId,
                        RoomId = roomId
                    }
                }
            };

            return await _roomRepository.AddAsync(room);
        }

        public async Task JoinRoomAsync(Guid roomId, Guid userId)
        {
            await _roomRepository.AddUserToRoomAsync(roomId, userId);
        }

        public async Task<List<Room>> GetUserRoomsAsync(Guid userId)
        {
            return await _roomRepository.GetRoomsByUserIdAsync(userId);
        }
    }
}
