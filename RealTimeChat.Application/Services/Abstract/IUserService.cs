using RealTimeChat.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Application.Services.Abstract
{
    // Kullanıcıyla ilgili işlemleri tanımlayan interface (sözleşme)
    public interface IUserService
    {
        // Tüm kullanıcıları listele
        Task<List<UserDto>> GetAllAsync();

        // Id ile kullanıcı getir (yoksa null döner)
        Task<UserDto?> GetByIdAsync(Guid id);

        // Yeni kullanıcı oluştur
        Task<UserDto> CreateAsync(UserDto userDto);
    }
}
