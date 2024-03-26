using Microsoft.AspNetCore.SignalR;

namespace ChatSender.Hubs
{


    public class ChatHub : Hub
    {


        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);




        }

    }
}
