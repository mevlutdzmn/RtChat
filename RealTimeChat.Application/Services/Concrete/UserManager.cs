using RealTimeChat.Application.DTOs;
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Domain.Entities;
using RealTimeChat.Infrastructure.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Application.Services.Concrete
{
    // Kullanıcı servisi: Repository üzerinden veriyle konuşur
    public class UserManager : IUserService
    {
        private readonly IUserRepository _userRepository;

        // Dependency Injection ile repository alınır
        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            // Entity → DTO dönüşümü
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            }).ToList();
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task<UserDto> CreateAsync(UserDto userDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = "dummy", // Gerçek hash ileride
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.AddAsync(user);

            return new UserDto
            {
                Id = created.Id,
                Username = created.Username,
                Email = created.Email
            };
        }
    }
}