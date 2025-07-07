using RealTimeChat.Application.DTOs;
using RealTimeChat.Application.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Application.Services.Concrete
{
    // IUserService'i gerçekleyen sınıf
    public class UserManager : IUserService
    {
        // Mock kullanıcı listesi (gerçek veri kaynağı yok henüz)
        private readonly List<UserDto> _users = new();

        // Tüm kullanıcıları getirir
        public async Task<List<UserDto>> GetAllAsync()
        {
            return await Task.FromResult(_users);
        }

        // Belirli bir Id’ye sahip kullanıcıyı getirir (yoksa null döner)
        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return await Task.FromResult(user);
        }

        // Yeni kullanıcı oluşturur ve listeye ekler
        public async Task<UserDto> CreateAsync(UserDto userDto)
        {
            userDto.Id = Guid.NewGuid(); // Yeni ID ata
            _users.Add(userDto); // Listeye ekle
            return await Task.FromResult(userDto);
        }
    }
}
