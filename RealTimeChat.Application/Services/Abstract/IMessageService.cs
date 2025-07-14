using RealTimeChat.Application.DTOs;
using RealTimeChat.Application.DTOs.Message;
using RealTimeChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Application.Services.Abstract
{
    // Mesajlarla ilgili işlemleri tanımlayan interface
    public interface IMessageService
    {
        // Tüm mesajları getir
        Task<List<MessageDto>> GetAllMessagesAsync();

        // Belirli bir kullanıcıya ait mesajları getir
        Task<List<MessageDto>> GetMessagesByUserAsync(Guid userId);

        // Yeni mesaj gönder
        Task<MessageDto> SendMessageAsync(MessageDto messageDto);

        Task SaveMessageAsync(SaveMessageDto dto);

        Task<List<Message>> GetMessagesBetweenUsersAsync(Guid userId1, Guid userId2);

    }
}
