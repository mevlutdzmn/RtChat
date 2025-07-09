using Microsoft.AspNetCore.Authentication.JwtBearer;    // JWT i�in gerekli namespace
using Microsoft.IdentityModel.Tokens;
using RealTimeChat.Application.DependencyInjection;      // Application katman�ndaki DI uzant�s�
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Application.Services.Concrete;
using RealTimeChat.Infrastructure.DependencyInjection;  // Infrastructure katman�ndaki DI uzant�s�
using RealTimeChat.Shared.Settings;                     // JWT ayarlar�n� almak i�in
using RealTimeChat.WebAPI.Hubs;                         // SignalR i�in Hub s�n�f�
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------
// 1. JWT Ayarlar�n� yap�land�r
// ------------------------------------------
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

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
builder.Services.AddApplication();                      // Application katman� ba��ml�l�klar�
builder.Services.AddInfrastructure(builder.Configuration); // Infrastructure katman� ba��ml�l�klar�
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

    // JWT header tan�m�
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'� buraya 'Bearer <token>' format�nda girin."
    });

    // Swagger'da her endpoint i�in token g�nderilmesini zorunlu k�lar
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
// 5. Di�er servisleri ekle
// ------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();


// ------------------------------------------
// 6. Pipeline ayarlar�
// ------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // JWT kimlik do�rulama
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
