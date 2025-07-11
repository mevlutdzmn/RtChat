using Microsoft.Extensions.DependencyInjection;
using RealTimeChat.Application.Security;
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Application.Services.Concrete;
using RealTimeChat.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChat.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserManager>();
            services.AddScoped<IMessageService, MessageManager>();
            services.AddScoped<ITokenService, TokenManager>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IRefreshTokenRepository, EfRefreshTokenRepository>();


            return services;
        }
    }
}
