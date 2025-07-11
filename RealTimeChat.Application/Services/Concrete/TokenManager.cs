using Microsoft.Extensions.Options;               // appsettings.json'daki ayarları almak için
using Microsoft.IdentityModel.Tokens;             // JWT token imzalama için
using RealTimeChat.Application.Services.Abstract; // ITokenService interface’i burada tanımlı
using RealTimeChat.Domain.Entities;               // User entity’si burada tanımlı
using RealTimeChat.Shared.Settings;               // JwtSettings sınıfı burada tanımlı
using System.IdentityModel.Tokens.Jwt;            // JWT token üretmek için
using System.Security.Claims;                     // Kullanıcı claim’leri (id, email vb.)
using System.Text;

namespace RealTimeChat.Application.Services.Concrete
{
    public class TokenManager : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        // JwtSettings'i appsettings.json'dan alıyoruz
        public TokenManager(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        // JWT token üretme metodu
        public string GenerateToken(User user)
        {
            // Kullanıcıya özel claim’ler (token’a gömülecek bilgiler)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Kullanıcının Id’si
                new Claim(ClaimTypes.Name, user.Username),                // Kullanıcının adı
                new Claim(ClaimTypes.Email, user.Email)                  // Kullanıcının emaili
            };

            // Secret key'den bir güvenlik anahtarı oluşturuyoruz
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            // Token’ı imzalamak için gerekli algoritma ve key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // JWT token nesnesi oluşturuluyor
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,           // appsettings.json → Issuer
                audience: _jwtSettings.Audience,       // appsettings.json → Audience
                claims: claims,                        // Kullanıcı bilgileri
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationMinutes), // Süresi
                signingCredentials: creds              // İmzalama bilgileri
            );

            // Token'ı string olarak döndür
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(User user)
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };
            return refreshToken;
        }

    }
}
