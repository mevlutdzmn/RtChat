//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using RealTimeChat.Application.Security;
//using System.Security.Cryptography;


//namespace RealTimeChat.Application.Services.Concrete
//{
//    public class PasswordHasher : IPasswordHasher
//    {
//        // Şifreyi SHA256 ile hashle
//        public string HashPassword(string password)
//        {
//            using var sha256 = SHA256.Create();
//            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
//            return Convert.ToBase64String(hashedBytes);
//        }

//        // Hash doğrulama
//        public bool VerifyPassword(string hashedPassword, string plainPassword)
//        {
//            var hashOfInput = HashPassword(plainPassword);
//            return hashOfInput == hashedPassword;
//        }
//    }
//}
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace RealTimeChat.Application.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Rastgele salt üret
            byte[] salt = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            // Şifreyi hashle
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        public bool VerifyPassword(string hashedPassword, string plainPassword)
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = parts[1];

            var hashToCompare = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: plainPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return storedHash == hashToCompare;
        }
    }
}
