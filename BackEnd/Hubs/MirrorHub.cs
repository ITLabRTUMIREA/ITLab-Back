using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BackEnd.Hubs
{
    public class MirrorHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}