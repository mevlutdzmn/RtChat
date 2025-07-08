using Microsoft.AspNetCore.SignalR;

namespace RealTimeChat.WebAPI.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            // Tüm kullanıcılara mesajı yayınla
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
