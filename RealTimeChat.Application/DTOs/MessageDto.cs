using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Application.DTOs
{
    // Mesaj bilgilerini API üzerinden göndermek veya almak için kullanılan veri transfer sınıfı
    public class MessageDto
    {
        public Guid Id { get; set; } // Mesajın benzersiz kimliği

        public string Content { get; set; } = null!; // Mesaj içeriği

        public DateTime SentAt { get; set; } // Mesajın gönderildiği tarih

        public Guid SenderId { get; set; } // Mesajı gönderen kullanıcının Id'si

        public string SenderUsername { get; set; } = null!; // Gönderen kullanıcının adı (dışarıdan görünür)
    }
}
