using Phishing_Detection.Models;
using Microsoft.AspNetCore.SignalR;

namespace Phishing_Detection.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", message);
        }
    }
}
