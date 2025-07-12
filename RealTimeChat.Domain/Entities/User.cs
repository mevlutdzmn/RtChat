using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } // Kullanıcının benzersiz kimliği

        public string Username { get; set; } = null!; // Kullanıcı adı

        public string Email { get; set; } = null!; // E-posta adresi

        public string PasswordHash { get; set; } = null!; // Şifre (hash haliyle)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Hesap oluşturulma zamanı

        public bool IsOnline { get; set; } = false; // Kullanıcı çevrimiçi mi?

        public ICollection<Message>? Messages { get; set; } // Kullanıcının gönderdiği mesajlar
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


        // email doğrulama 
        public bool EmailConfirmed { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpires { get; set; }

    }
}
