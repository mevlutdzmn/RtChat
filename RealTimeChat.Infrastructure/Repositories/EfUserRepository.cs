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
    // Entity Framework kullanarak kullanıcı verilerini yöneten somut (concrete) repository sınıfı
    public class EfUserRepository : IUserRepository
    {
        private readonly RealTimeChatDbContext _context;  // DbContext nesnesi, veri tabanı işlemleri için

        // DbContext nesnesi constructor ile dependency injection yoluyla alınır
        public EfUserRepository(RealTimeChatDbContext context)
        {
            _context = context;  // DbContext örneğini sınıf içinde sakla
        }

        // Tüm kullanıcıları veritabanından asenkron olarak liste halinde getirir
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
            // _context.Users -> Users tablosunu temsil eder
            // ToListAsync() -> verileri listeye çevirip asenkron getirir
        }

        // Verilen Id'ye sahip kullanıcıyı veritabanından asenkron olarak getirir
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            // FirstOrDefaultAsync: Koşulu sağlayan ilk kullanıcıyı getirir, yoksa null döner
            // u => u.Id == id : LINQ lambda ifadesi, Id eşleşmesini kontrol eder
        }

        // Yeni kullanıcıyı veritabanına ekler ve eklenen kullanıcıyı geri döner
        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user); // Kullanıcı nesnesini EF Core değişiklik takip mekanizmasına ekle
            await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet (asenkron)
            return user; // Eklenen kullanıcıyı geri döndür
        }
    }
}
