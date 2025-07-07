
using Microsoft.EntityFrameworkCore;  // EF Core fonksiyonları (Include, ToListAsync vs.)
using RealTimeChat.Domain.Entities;   // Message ve User gibi entity sınıfları için
using RealTimeChat.Domain.Repositories;
using RealTimeChat.Infrastructure.Context;  // DbContext sınıfı (veritabanı bağlantısı)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Infrastructure.Repositories
{
    // Entity Framework kullanarak mesaj verilerini yöneten repository sınıfı
    public class EfMessageRepository : IMessageRepository
    {
        private readonly RealTimeChatDbContext _context; // Veritabanı işlemleri için EF Core context nesnesi

        // Constructor: DbContext dependency injection ile alınır
        public EfMessageRepository(RealTimeChatDbContext context)
        {
            _context = context;
        }

        // Tüm mesajları, gönderen kullanıcı bilgisiyle birlikte liste olarak getirir
        public async Task<List<Message>> GetAllAsync()
        {
            return await _context.Messages
                .Include(m => m.Sender) // Her mesajın gönderen (User) bilgisi de dahil edilsin
                .ToListAsync();         // Asenkron olarak listeye dönüştür
        }

        // Belirli bir kullanıcıya ait mesajları getirir (kimin gönderdiğine göre filtreleme yapar)
        public async Task<List<Message>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Messages
                .Include(m => m.Sender)                 // Gönderen kullanıcı bilgisiyle birlikte
                .Where(m => m.SenderId == userId)       // Sadece bu kullanıcıya ait mesajlar
                .ToListAsync();                         // Asenkron olarak listele
        }

        // Yeni mesajı veritabanına ekler ve başarılıysa eklenen mesajı döner
        public async Task<Message> AddAsync(Message message)
        {
            _context.Messages.Add(message);           // EF Core'un takip sistemine mesajı ekle
            await _context.SaveChangesAsync();        // Değişiklikleri veritabanına kaydet
            return message;                           // Eklenen mesajı geri döndür
        }
    }
}
