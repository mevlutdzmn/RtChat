using Microsoft.AspNetCore.Authentication.JwtBearer;    // JWT için gerekli namespace
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealTimeChat.Application.DependencyInjection;      // Application katmanındaki DI uzantısı
using RealTimeChat.Application.Security;
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Application.Services.Concrete;
using RealTimeChat.Domain.Entities;
using RealTimeChat.Domain.Repositories;
using RealTimeChat.Infrastructure.DependencyInjection;  // Infrastructure katmanındaki DI uzantısı
using RealTimeChat.Infrastructure.Email;
using RealTimeChat.Infrastructure.Repositories;
using RealTimeChat.Shared.Settings;                     // JWT ayarlarını almak için
using RealTimeChat.WebAPI.Hubs;                         // SignalR için Hub sınıfı
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------
// 1. JWT Ayarlarını yapılandır
// ------------------------------------------
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowChatFrontend", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:8080", "http://localhost:8080")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // ⚠️ SignalR için gerekli
    });
});




// ------------------------------------------
// 2. JWT Authentication
// ------------------------------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };

    // ✅ SignalR için access_token'ı query'den çek
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});


// ------------------------------------------
// 3. Katman servislerini ekle (IoC)
// ------------------------------------------
builder.Services.AddApplication();                      // Application katmanı bağımlılıkları
builder.Services.AddInfrastructure(builder.Configuration); // Infrastructure katmanı bağımlılıkları
builder.Services.AddScoped<ITokenService, TokenManager>(); // Token servisi
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<MailHelper>();
builder.Services.AddScoped<IRoomService, RoomManager>();
builder.Services.AddScoped<IRoomRepository, EFRoomRepository>();





// ------------------------------------------
// 4. Swagger + JWT destekli
// ------------------------------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RealTimeChat API",
        Version = "v1"
    });

    // JWT header tanımı
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'ı buraya 'Bearer <token>' formatında girin."
    });

    // Swagger'da her endpoint için token gönderilmesini zorunlu kılar
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ------------------------------------------
// 5. Diğer servisleri ekle
// ------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();



// ------------------------------------------
// 6. Pipeline ayarları
// ------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowChatFrontend"); // ✅ tanımladığın isimle birebir aynı olmalı

app.UseAuthentication(); // JWT kimlik doğrulama
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");



app.Run();
