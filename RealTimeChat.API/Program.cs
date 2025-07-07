using RealTimeChat.Application.DependencyInjection;
using RealTimeChat.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Servis kayýtlarý (katmanlara daðýlmýþ þekilde)
builder.Services.AddApplication(); // Application katmanýndan
builder.Services.AddInfrastructure(builder.Configuration); // Infrastructure katmanýndan

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
