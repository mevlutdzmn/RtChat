using RealTimeChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Infrastructure.Repositories.Abstract
{
    // Mesaja ait temel veri erişim işlemleri
    public interface IMessageRepository
    {
        Task<List<Message>> GetAllAsync();

        Task<List<Message>> GetByUserIdAsync(Guid userId);

        Task<Message> AddAsync(Message message);
    }
}
