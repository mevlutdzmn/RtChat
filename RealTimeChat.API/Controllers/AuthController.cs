using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Application.DTOs.Auth;
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Domain.Entities;
using RealTimeChat.Domain.Repositories;
using System.Threading.Tasks;

namespace RealTimeChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthController(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        // Kullanıcı girişi işlemi
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            if (user.PasswordHash != request.Password) // ❗ Şifre hash kontrolü yapılmalı, bu sadece demo
                return Unauthorized("Şifre yanlış.");

            var token = _tokenService.GenerateToken(user);

            return Ok(new { Token = token });
        }

        // Yeni kullanıcı kaydı
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("Bu email zaten kayıtlı.");

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password, // ❗ Hashleme yok, güvenli değil
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(newUser);

            var token = _tokenService.GenerateToken(newUser);

            return Ok(new { Token = token });
        }
    }
}
