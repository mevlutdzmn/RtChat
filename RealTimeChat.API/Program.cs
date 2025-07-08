using RealTimeChat.Application.DependencyInjection;      // Application katmanýndaki DI uzantýsý
using RealTimeChat.Infrastructure.DependencyInjection;  // Infrastructure katmanýndaki DI uzantýsý
using RealTimeChat.WebAPI.Hubs;                         // SignalR için Hub sýnýfý
using Microsoft.AspNetCore.Authentication.JwtBearer;    // JWT için gerekli namespace
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RealTimeChat.Shared.Settings;                     // JWT ayarlarýný almak için

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------
// 1. JWT Ayarlarýný yapýlandýr
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
builder.Services.AddApplication();                      // Application katmaný baðýmlýlýklarý
builder.Services.AddInfrastructure(builder.Configuration); // Infrastructure katmaný baðýmlýlýklarý

// **Eksik olabilir: Eðer AddApplication() içinde ITokenService yoksa burada ekle**
// Örnek:
// builder.Services.AddScoped<ITokenService, TokenService>();

// ------------------------------------------
// 4. Diðer servisleri ekle
// ------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// ------------------------------------------
// 5. Ortam kontrolü ve middleware'ler
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
