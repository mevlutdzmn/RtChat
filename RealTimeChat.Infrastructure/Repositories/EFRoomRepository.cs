using Microsoft.EntityFrameworkCore;
using RealTimeChat.Domain.Entities;
using RealTimeChat.Domain.Repositories;
using RealTimeChat.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Infrastructure.Repositories
{
    public class EFRoomRepository : IRoomRepository
    {
        private readonly RealTimeChatDbContext _context;

        public EFRoomRepository(RealTimeChatDbContext context)
        {
            _context = context;
        }

        public async Task<Room> AddAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;  // EKLE: kaydedilen entity'i döndür
        }


        public async Task<List<Room>> GetAllAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(Guid id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(string roomName)
        {
            return await _context.Rooms.AnyAsync(r => r.Name == roomName);
        }


        public async Task AddUserToRoomAsync(Guid userId, Guid roomId)
        {
            var userRoom = new UserRoom
            {
                UserId = userId,
                RoomId = roomId
            };

            _context.UserRooms.Add(userRoom);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Room>> GetRoomsByUserIdAsync(Guid userId)
        {
            return await _context.UserRooms
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Room)
                .ToListAsync();
        }


    }
}
