using RealTimeChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Domain.Repositories
{
    public interface IRoomRepository
    {
        Task<List<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(Guid id);
        Task<Room> AddAsync(Room room);
        Task<bool> ExistsAsync(string roomName);
        Task AddUserToRoomAsync(Guid userId, Guid roomId);
        Task<List<Room>> GetRoomsByUserIdAsync(Guid userId);

    }
}
