using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Application.DTOs.Auth;                 // DTO'ları kullanmak için
using RealTimeChat.Application.Security;                 // Şifre hashleme arayüzü
using RealTimeChat.Application.Services.Abstract;         // Token servisi
using RealTimeChat.Domain.Entities;                      // User entity
using RealTimeChat.Domain.Repositories;                  // Kullanıcı repository
using System.Threading.Tasks;

namespace RealTimeChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;     // Veritabanı üzerinden kullanıcı işlemleri
        private readonly ITokenService _tokenService;         // JWT token üretme servisi
        private readonly IPasswordHasher _passwordHasher;     // Şifre hashleme servisi

        // Controller'ın constructor'ı - Bağımlılıklar burada enjekte edilir
        public AuthController(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        //  Kullanıcı girişi (Login)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Email ile kullanıcıyı bul
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Şifre doğrulaması (hash karşılaştırması)
            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
                return Unauthorized("Şifre yanlış.");

            // Token oluştur
            var token = _tokenService.GenerateToken(user);

            return Ok(new { Token = token });
        }

        //  Kullanıcı kaydı (Register)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            // Daha önce bu email ile kullanıcı var mı?
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("Bu email zaten kayıtlı.");

            // Yeni kullanıcı nesnesi oluştur
            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password), //  Şifreyi hashle
                CreatedAt = DateTime.UtcNow
            };

            // Veritabanına kaydet
            await _userRepository.AddAsync(newUser);

            // Token üret ve döndür
            var token = _tokenService.GenerateToken(newUser);

            return Ok(new { Token = token });
        }
    }
}
