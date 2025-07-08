using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Application.DTOs.Auth; // DTO'ları buradan alıyoruz
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Domain.Entities;
using RealTimeChat.Domain.Repositories;
using System.Threading.Tasks;

namespace RealTimeChat.API.Controllers
{
    [ApiController] // Bu sınıfın bir Web API Controller olduğunu belirtir
    [Route("api/[controller]")] // İsteklerin "api/auth" şeklinde yönlendirilmesini sağlar
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository; // Kullanıcı verilerini çekeceğimiz repository
        private readonly ITokenService _tokenService;     // Token üretmek için kullanılan servis

        // Constructor'da bağımlılıkları enjekte ediyoruz
        public AuthController(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            if (user.PasswordHash != request.Password)
                return Unauthorized("Şifre yanlış.");

            var token = _tokenService.GenerateToken(user);

            return Ok(new { Token = token });
        }

        // Register Endpoint'i
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            // Email zaten var mı kontrol et
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("Bu email zaten kayıtlı.");

            // Yeni kullanıcı oluştur
            var newUser = new User
            {
                Username = request.Username,          // Username atandı
                Email = request.Email,
                PasswordHash = request.Password,      // Şifreyi hashlemeyi unutma!
            };

            // Kullanıcı ekle
            await _userRepository.AddAsync(newUser);

            // Token üret
            var token = _tokenService.GenerateToken(newUser);

            return Ok(new { Token = token });
        }

    }
}
