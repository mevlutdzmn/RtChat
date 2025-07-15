using RealTimeChat.Application.DTOs.Room;
using RealTimeChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Application.Services.Abstract
{
    public interface IRoomService
    {
        Task<Room> CreateRoomAsync(CreateRoomDto dto, Guid creatorUserId);
        Task JoinRoomAsync(Guid roomId, Guid userId);
        Task<List<Room>> GetUserRoomsAsync(Guid userId);
    }
}
