using Microsoft.AspNetCore.SignalR;
using RealTimeChat.Application.DTOs.Message;
using RealTimeChat.Application.Services.Abstract;
using System.Security.Claims;

namespace RealTimeChat.API.Hubs
{
    public class MessageHub : Hub
    {
        private readonly IMessageService _messageService;

        public MessageHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessageAsync(string receiverId, string messageContent)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId) || string.IsNullOrEmpty(messageContent))
                return;

            // 1. Mesajı DB’ye kaydet
            await _messageService.SaveMessageAsync(new SaveMessageDto
            {
                SenderId = Guid.Parse(senderId),
                ReceiverId = Guid.Parse(receiverId),
                Content = messageContent
            });

            // 2. Karşı tarafa mesajı gönder
            await Clients.User(receiverId).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                Content = messageContent,
                SentAt = DateTime.UtcNow
            });
        }
    }

}
