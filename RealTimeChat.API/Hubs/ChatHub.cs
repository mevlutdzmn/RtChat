using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace RealTimeChat.WebAPI.Hubs
{
    [Authorize] // SignalR bağlantısı için JWT zorunlu
    public class ChatHub : Hub
    {
        // Kullanıcı bağlandığında
        public override async Task OnConnectedAsync()
        {
            var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            Console.WriteLine($"{userEmail} bağlandı.");
            await base.OnConnectedAsync();
        }

        // Kullanıcı ayrıldığında
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userEmail = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            Console.WriteLine($"{userEmail} ayrıldı.");
            await base.OnDisconnectedAsync(exception);
        }

        // Mesaj gönderme metodu
        public async Task SendMessage(string message)
        {
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonim";
            await Clients.All.SendAsync("ReceiveMessage", username, message);
        }
    }
}
