using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeChat.Application.DTOs;
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.WebAPI.Hubs;

namespace RealTimeChat.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;

        // Tek constructor, iki dependency injection parametresi alıyor
        public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _messageService.GetAllMessagesAsync();
            return Ok(messages);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var messages = await _messageService.GetMessagesByUserAsync(userId);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] MessageDto messageDto)
        {
            var sent = await _messageService.SendMessageAsync(messageDto);

            // Mesaj gönderildikten sonra SignalR ile anlık yayın yap
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", messageDto.SenderUsername, messageDto.Content);

            return Ok(sent);
        }
    }
}
