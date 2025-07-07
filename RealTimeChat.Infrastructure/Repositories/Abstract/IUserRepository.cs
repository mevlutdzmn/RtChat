using RealTimeChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Infrastructure.Repositories.Abstract
{
    // Kullanıcıya ait temel veri erişim işlemleri
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(Guid id);

        Task<User> AddAsync(User user);
    }
}
