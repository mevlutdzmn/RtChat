using RealTimeChat.Application.DependencyInjection;
using RealTimeChat.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Servis kay�tlar� (katmanlara da��lm�� �ekilde)
builder.Services.AddApplication(); // Application katman�ndan
builder.Services.AddInfrastructure(builder.Configuration); // Infrastructure katman�ndan

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
