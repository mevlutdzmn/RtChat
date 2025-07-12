using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RealTimeChat.Application.DTOs.Auth;
using RealTimeChat.Application.Security;
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Domain.Entities;
using RealTimeChat.Domain.Repositories;
using System.Security.Claims;
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
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthController(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
        }

        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
                return Unauthorized("Şifre yanlış.");

            if (!user.EmailConfirmed)
                return Unauthorized("E-posta doğrulanmamış. Lütfen e-posta adresinizi doğrulayın.");

            var accessToken = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user);
            await _refreshTokenRepository.AddAsync(refreshToken);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            });
        }



        /// <summary>
        /// Yeni kullanıcı kaydı
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("Bu email zaten kayıtlı.");

            var emailToken = Guid.NewGuid().ToString();

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                EmailVerificationToken = emailToken,
                EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(1),
                EmailConfirmed = false
            };

            await _userRepository.AddAsync(newUser);

            var accessToken = _tokenService.GenerateToken(newUser);
            var refreshToken = _tokenService.GenerateRefreshToken(newUser);
            await _refreshTokenRepository.AddAsync(refreshToken);

            // 👉 Email doğrulama linkini simüle edelim
            Console.WriteLine($"✅ E-posta doğrulama linki: https://localhost:7018/api/auth/verify-email?token={emailToken}");

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Message = "Kayıt başarılı. Lütfen email adresinizi doğrulayın."
            });
        }



        /// <summary>
        /// Giriş yapan kullanıcının bilgilerini döner
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Token geçersiz.");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Refresh token boş olamaz.");

            var stored = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (stored == null || stored.ExpiresAt < DateTime.UtcNow || stored.IsRevoked)
                return Unauthorized("Geçersiz ya da süresi dolmuş token.");

            // Yeni token’ları oluştur
            var newAccessToken = _tokenService.GenerateToken(stored.User);
            var newRefreshToken = _tokenService.GenerateRefreshToken(stored.User);

            // Eski token’ı iptal et
            await _refreshTokenRepository.RevokeAsync(refreshToken);

            // Yeni refresh token’ı veritabanına kaydet
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }


        //Doğrulama endpointi
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var user = await _userRepository.GetByEmailVerificationTokenAsync(token);

            if (user == null || user.EmailVerificationTokenExpires < DateTime.UtcNow)
                return BadRequest("Geçersiz ya da süresi dolmuş token.");

            user.EmailConfirmed = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpires = null;

            await _userRepository.UpdateAsync(user);

            return Ok("✅ Email başarıyla doğrulandı.");
        }



    }
}
