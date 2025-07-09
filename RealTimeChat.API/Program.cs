using Microsoft.AspNetCore.Authentication.JwtBearer;    // JWT için gerekli namespace
using Microsoft.IdentityModel.Tokens;
using RealTimeChat.Application.DependencyInjection;      // Application katmanýndaki DI uzantýsý
using RealTimeChat.Application.Services.Abstract;
using RealTimeChat.Application.Services.Concrete;
using RealTimeChat.Infrastructure.DependencyInjection;  // Infrastructure katmanýndaki DI uzantýsý
using RealTimeChat.Shared.Settings;                     // JWT ayarlarýný almak için
using RealTimeChat.WebAPI.Hubs;                         // SignalR için Hub sýnýfý
using System.Text;
using Microsoft.OpenApi.Models;

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

    // JWT header tanýmý
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'ý buraya 'Bearer <token>' formatýnda girin."
    });

    // Swagger'da her endpoint için token gönderilmesini zorunlu kýlar
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
// 5. Diðer servisleri ekle
// ------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();


// ------------------------------------------
// 6. Pipeline ayarlarý
// ------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // JWT kimlik doðrulama
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
