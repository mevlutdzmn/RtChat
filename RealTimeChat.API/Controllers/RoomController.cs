using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Application.DTOs.Room;
using RealTimeChat.Application.Services.Abstract;
using System.Security.Claims;

namespace RealTimeChat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var room = await _roomService.CreateRoomAsync(dto, userId);
            return Ok(room);
        }

        [Authorize]
        [HttpPost("join/{roomId}")]
        public async Task<IActionResult> JoinRoom(Guid roomId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _roomService.JoinRoomAsync(roomId, userId);
            return Ok("Odaya katılım başarılı.");
        }

        [Authorize]
        [HttpGet("my-rooms")]
        public async Task<IActionResult> GetMyRooms()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var rooms = await _roomService.GetUserRoomsAsync(userId);
            return Ok(rooms);
        }
    }
}
