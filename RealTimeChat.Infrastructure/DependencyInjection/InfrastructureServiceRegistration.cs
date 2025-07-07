using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealTimeChat.Domain.Repositories;
using RealTimeChat.Infrastructure.Context;
using RealTimeChat.Infrastructure.Repositories;

namespace RealTimeChat.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Veritabanı bağlantısını ekle
            services.AddDbContext<RealTimeChatDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlServer")));

            // Repository'leri ekle
            services.AddScoped<IUserRepository, EfUserRepository>();
            services.AddScoped<IMessageRepository, EfMessageRepository>();

            return services;
        }
    }
}
