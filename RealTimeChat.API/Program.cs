using Microsoft.EntityFrameworkCore;
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Application.Services.Concrete;
using RealTimeChat.Infrastructure.Context;
using RealTimeChat.Infrastructure.Repositories.Abstract;
using RealTimeChat.Infrastructure.Repositories.Concrete;

var builder = WebApplication.CreateBuilder(args);

// InMemory DbContext (þimdilik, ileride SqlServer yapacaðýz)
builder.Services.AddDbContext<RealTimeChatDbContext>(options =>
    options.UseInMemoryDatabase("ChatDb"));

// Servisler ve Repositories
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IMessageService, MessageManager>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IMessageRepository, EfMessageRepository>();

// Controller ve Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
