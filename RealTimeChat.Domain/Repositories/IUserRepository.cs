using RealTimeChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Domain.Repositories
{
    // Kullanıcıya ait temel veri erişim işlemleri
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(Guid id);

        Task<User> AddAsync(User user);
    }
}
