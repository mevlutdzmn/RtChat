using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; } // Mesaj kimliği (PK)

        public string Content { get; set; } = null!; // Mesaj içeriği

        public DateTime SentAt { get; set; } = DateTime.UtcNow; // Gönderilme zamanı

        public Guid SenderId { get; set; } // Gönderen kullanıcı (FK)

        public User Sender { get; set; } = null!; // Navigasyon: mesajı atan kullanıcı
    }
}
