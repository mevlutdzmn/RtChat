using Microsoft.AspNetCore.Authentication.JwtBearer;    // JWT için gerekli namespace
using Microsoft.IdentityModel.Tokens;
using RealTimeChat.Application.DependencyInjection;      // Application katmanındaki DI uzantısı
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Application.Services.Concrete;
using RealTimeChat.Infrastructure.DependencyInjection;  // Infrastructure katmanındaki DI uzantısı
using RealTimeChat.Shared.Settings;                     // JWT ayarlarını almak için
using RealTimeChat.WebAPI.Hubs;                         // SignalR için Hub sınıfı
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------
// 1. JWT Ayarlarını yapılandır
// ------------------------------------------
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

var allowedOrigins = new string[]
{
   
    "http://127.0.0.1:5500"
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy.WithOrigins("https://localhost:3000") // frontend URL'si
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
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
});

// ------------------------------------------
// 3. Katman servislerini ekle (IoC)
// ------------------------------------------
builder.Services.AddApplication();                      // Application katmanı bağımlılıkları
builder.Services.AddInfrastructure(builder.Configuration); // Infrastructure katmanı bağımlılıkları
builder.Services.AddScoped<ITokenService, TokenManager>(); // Token servisi

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
app.UseCors("default");

app.UseAuthentication(); // JWT kimlik doğrulama
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");



app.Run();
