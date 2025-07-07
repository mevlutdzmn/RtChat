using Microsoft.EntityFrameworkCore;
using RealTimeChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Infrastructure.Context
{
    public class RealTimeChatDbContext : DbContext
    {
        // Constructor: dışarıdan gelen options parametresi ile konfigürasyon yapılır
        public RealTimeChatDbContext(DbContextOptions<RealTimeChatDbContext> options) : base(options)
        {
        }

        // Veritabanı tablolarını temsil eden DbSet'ler
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;

        // Fluent API konfigürasyonları (ileride burada yapılacak)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User → Message ilişkisi (1:N)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
