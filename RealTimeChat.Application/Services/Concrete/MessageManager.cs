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
    // IMessageService arayüzünü implemente eden servis sınıfı
    // Mesajlara dair iş kurallarını ve veri erişimini yönetir
    public class MessageManager : IMessageService
    {
        private readonly IMessageRepository _messageRepository; // Mesaj veri erişimi için repository
        private readonly IUserRepository _userRepository;       // Kullanıcı veri erişimi için repository

        // Constructor: Repository nesneleri dependency injection ile alınır
        public MessageManager(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        // Tüm mesajları DTO olarak getirir
        public async Task<List<MessageDto>> GetAllMessagesAsync()
        {
            var messages = await _messageRepository.GetAllAsync();
            // Veritabanından tüm mesajları asenkron olarak çek

            return messages.Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderId = m.SenderId,
                SenderUsername = m.Sender?.Username ?? "Bilinmiyor" // Gönderen kullanıcı adı, null ise "Bilinmiyor"
            }).ToList();
            // Entity'den DTO'ya dönüştürme yap ve liste olarak döndür
        }

        // Belirli kullanıcıya ait mesajları DTO olarak getirir
        public async Task<List<MessageDto>> GetMessagesByUserAsync(Guid userId)
        {
            var messages = await _messageRepository.GetByUserIdAsync(userId);
            // Veritabanından userId'ye göre mesajları çek

            return messages.Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderId = m.SenderId,
                SenderUsername = m.Sender?.Username ?? "Bilinmiyor"
            }).ToList();
            // Entity'den DTO'ya dönüştür ve listeyi döndür
        }

        // Yeni mesaj gönderme işlemi
        public async Task<MessageDto> SendMessageAsync(MessageDto messageDto)
        {
            // Mesajı gönderen kullanıcıyı veritabanından getir
            var user = await _userRepository.GetByIdAsync(messageDto.SenderId);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı"); // Kullanıcı yoksa hata fırlat

            // Yeni Message entity'si oluştur
            var message = new Message
            {
                Id = Guid.NewGuid(),           // Yeni benzersiz ID oluştur
                Content = messageDto.Content,  // Mesaj içeriği
                SentAt = DateTime.UtcNow,      // Şu anki zamanı UTC olarak ata
                SenderId = user.Id,            // Gönderen kullanıcı ID'si
                Sender = user                  // Gönderen kullanıcı objesi
            };

            // Mesajı veritabanına ekle
            var added = await _messageRepository.AddAsync(message);

            // Veritabanına eklenen mesajı DTO'ya dönüştür ve döndür
            return new MessageDto
            {
                Id = added.Id,
                Content = added.Content,
                SentAt = added.SentAt,
                SenderId = added.SenderId,
                SenderUsername = added.Sender?.Username ?? "Bilinmiyor"
            };
        }
    }

}
