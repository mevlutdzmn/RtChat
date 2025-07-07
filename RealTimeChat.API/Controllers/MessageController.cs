using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Application.DTOs;
using RealTimeChat.Application.Services.Abstract;

namespace RealTimeChat.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
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
            return Ok(sent);
        }
    }
}
