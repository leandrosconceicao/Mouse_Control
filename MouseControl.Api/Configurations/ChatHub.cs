using Microsoft.AspNetCore.SignalR;
using MouseControl.Domain.Entities;
using MouseControl.Domain.Interfaces;

namespace MouseControl.Api.Configurations
{
    public class ChatHub : Hub<IChatHub>
    {
        public async void SendMessage(MousePosition position)
        {
            await Clients.All.SetPosition(position);
        }

        public async void LeftClick() => await Clients.All.LeftClick();

        public async void RightClick() => await Clients.All.RightClick();

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }
    }
}
