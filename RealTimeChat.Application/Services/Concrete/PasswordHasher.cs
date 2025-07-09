using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealTimeChat.Application.Security;
using System.Security.Cryptography;


namespace RealTimeChat.Application.Services.Concrete
{
    public class PasswordHasher : IPasswordHasher
    {
        // Şifreyi SHA256 ile hashle
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Hash doğrulama
        public bool VerifyPassword(string hashedPassword, string plainPassword)
        {
            var hashOfInput = HashPassword(plainPassword);
            return hashOfInput == hashedPassword;
        }
    }
}
