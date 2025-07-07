using RealTimeChat.Application.DTOs;
using RealTimeChat.Application.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Application.Services.Concrete
{
    // IMessageService'i gerçekleyen sınıf
    public class MessageManager : IMessageService
    {
        // Mock mesaj listesi
        private readonly List<MessageDto> _messages = new();

        // Tüm mesajları getirir
        public async Task<List<MessageDto>> GetAllMessagesAsync()
        {
            return await Task.FromResult(_messages);
        }

        // Belirli bir kullanıcıya ait mesajları getirir
        public async Task<List<MessageDto>> GetMessagesByUserAsync(Guid userId)
        {
            var userMessages = _messages.Where(m => m.SenderId == userId).ToList();
            return await Task.FromResult(userMessages);
        }

        // Yeni mesajı listeye ekler ve geri döner
        public async Task<MessageDto> SendMessageAsync(MessageDto messageDto)
        {
            messageDto.Id = Guid.NewGuid(); // Yeni Id ata
            messageDto.SentAt = DateTime.UtcNow; // Zaman ata
            _messages.Add(messageDto); // Listeye ekle
            return await Task.FromResult(messageDto);
        }
    }
}
