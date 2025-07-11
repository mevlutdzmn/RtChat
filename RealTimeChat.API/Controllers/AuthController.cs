using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Application.DTOs.Auth;       // DTO'lar
using RealTimeChat.Application.Security;       // IPasswordHasher
using RealTimeChat.Application.Services.Abstract; // ITokenService
using RealTimeChat.Domain.Entities;            // User entity
using RealTimeChat.Domain.Repositories;        // IUserRepository
using System;
using System.Threading.Tasks;

namespace RealTimeChat.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthController(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
                return Unauthorized("Şifre yanlış.");

            var token = _tokenService.GenerateToken(user);

            return Ok(new { Token = token });
        }

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
                CreatedAt = DateTime.UtcNow,
                PasswordHash = _passwordHasher.HashPassword(request.Password)  // Burada hashleme yapılıyor
            };

            await _userRepository.AddAsync(newUser);

            var token = _tokenService.GenerateToken(newUser);

            return Ok(new { Token = token });
        }
    }
}
