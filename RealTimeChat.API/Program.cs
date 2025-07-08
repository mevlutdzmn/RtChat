using RealTimeChat.Application.DependencyInjection;      // Application katman�ndaki DI uzant�s�
using RealTimeChat.Infrastructure.DependencyInjection;  // Infrastructure katman�ndaki DI uzant�s�
using RealTimeChat.WebAPI.Hubs;                         // SignalR i�in Hub s�n�f�
using Microsoft.AspNetCore.Authentication.JwtBearer;    // JWT i�in gerekli namespace
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RealTimeChat.Shared.Settings;                     // JWT ayarlar�n� almak i�in

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

// **Eksik olabilir: E�er AddApplication() i�inde ITokenService yoksa burada ekle**
// �rnek:
// builder.Services.AddScoped<ITokenService, TokenService>();

// ------------------------------------------
// 4. Di�er servisleri ekle
// ------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// ------------------------------------------
// 5. Ortam kontrol� ve middleware'ler
// ------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
