using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RealTimeChat.Application.DTOs.Auth;
using RealTimeChat.Application.Security;
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Domain.Entities;
using RealTimeChat.Domain.Repositories;
using RealTimeChat.Infrastructure.Email;
using System.Security.Claims;

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
        private readonly MailHelper _mailHelper;

        public AuthController(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IRefreshTokenRepository refreshTokenRepository,
            MailHelper mailHelper)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _mailHelper = mailHelper;
        }

        // ✅ 1. Kullanıcı Kaydı
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

            // ✅ Gerçek mail gönderimi
            string verificationLink = $"https://localhost:7018/api/auth/verify-email?token={emailToken}";
            await _mailHelper.SendEmailAsync(
                request.Email,
                "Email Doğrulama",
                $"<p>Merhaba {request.Username},</p><p>Hesabınızı doğrulamak için <a href='{verificationLink}'>buraya tıklayın</a>.</p>"
            );

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Message = "Kayıt başarılı. Lütfen email adresinizi kontrol edin."
            });
        }

        // ✅ 2. Kullanıcı Girişi
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

        // ✅ 3. Refresh Token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Refresh token boş olamaz.");

            var stored = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (stored == null || stored.ExpiresAt < DateTime.UtcNow || stored.IsRevoked)
                return Unauthorized("Geçersiz ya da süresi dolmuş token.");

            var newAccessToken = _tokenService.GenerateToken(stored.User);
            var newRefreshToken = _tokenService.GenerateRefreshToken(stored.User);

            await _refreshTokenRepository.RevokeAsync(refreshToken);
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        // ✅ 4. Kimlik Bilgisi
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

        // ✅ 5. E-posta Doğrulama
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

        // ✅ 5. parolamı unuttum
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return BadRequest("Bu e-posta adresi kayıtlı değildir.");
            }

            var resetToken = Guid.NewGuid().ToString();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateAsync(user);

            var resetLink = $"https://localhost:7018/reset-password?token={resetToken}";
            await _mailHelper.SendEmailAsync(user.Email, "Şifre Sıfırlama", $"Şifre sıfırlamak için tıklayın: <a href='{resetLink}'>Şifreyi Sıfırla</a>");

            return Ok("Şifre sıfırlama maili gönderildi.");
        }

        //reset password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _userRepository.GetByPasswordResetTokenAsync(dto.Token);

            if (user == null || user.PasswordResetTokenExpires < DateTime.UtcNow)
                return BadRequest("Geçersiz ya da süresi dolmuş token.");

            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;

            await _userRepository.UpdateAsync(user);

            return Ok("✅ Şifreniz başarıyla güncellendi.");
        }



    }
}
