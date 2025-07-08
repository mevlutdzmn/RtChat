using RealTimeChat.Application.DTOs.Auth;
using RealTimeChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Application.Services.Abstract
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDto dto);
        Task<User> RegisterAsync(RegisterDto dto);
    }
}
