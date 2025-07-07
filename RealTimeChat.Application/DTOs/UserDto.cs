using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Application.DTOs
{
    // Kullanıcıyla ilgili dış dünyaya (API vs.) döneceğimiz verileri içeren sınıf
    public class UserDto
    {
        public Guid Id { get; set; } // Kullanıcının kimliği

        public string Username { get; set; } = null!; // Kullanıcının adı

        public string Email { get; set; } = null!; // Kullanıcının e-posta adresi
    }
}
