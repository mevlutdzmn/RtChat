using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Application.DTOs;
using RealTimeChat.Application.Services.Abstract;

namespace RealTimeChat.WebAPI.Controllers
{
    [Authorize] // Tüm controller'ı yetkili kullanıcıya sınırlar
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto userDto)
        {
            var created = await _userService.CreateAsync(userDto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
    }
}
