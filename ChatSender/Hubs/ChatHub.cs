using ChatSender.Core.Externals;
using ChatSender.Core.Models;
using ChatSender.Core.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatSender.Hubs
{


    public class ChatHub : Hub
    {
        private readonly GlobalService _globalService;
        private readonly IChatService _chatService;

        private ILogger<ChatHub> _logger;

        public ChatHub(GlobalService globalService, IChatService chatService, ILogger<ChatHub> logger) 
        {
            _globalService = globalService;
            _chatService = chatService;
            _logger = logger;
        }

        public async Task<Message> SendMessage(Message msg)
        {
            try 
            {
                var user = _globalService.GetUser(Context.ConnectionId);
                var lastSentTime = _globalService.GetUserLastSentTime(Context.ConnectionId);


                //validate message and make return message
                var retMessage = await _chatService.MakeReturnMessage(msg, user, lastSentTime);
                _globalService.UpdateUserLastSentTime(Context.ConnectionId);


                //send message
                var groupUsers = await _chatService.GetGroup(msg.ChannelId).GetUsers();

                foreach (var u in groupUsers) 
                {
                    if (u.Id == user.Id) continue;

                    //if the receiver u is connected to the current server, then send the message directly
                    //otherwise, broadcast this message to other servers
                    try
                    {
                        var uConnId = _globalService.GetConnectionId(u.Id);
                        await Clients.Client(uConnId).SendAsync("ReceiveMessage", retMessage);
                    }
                    catch (InvalidDataException e) 
                    {
                        //to be implemented
                    }
                }


                return retMessage;
            
            }catch (Exception ex) 
            {
                _logger.LogDebug("User invalid message");
                _logger.LogDebug(ex.ToString());
            }

            return null;
        }

        public async Task<bool> SendAuthUser(string token)
        {
            try
            {
                var user = await _chatService.GetUserByToken(token);
                _globalService.OnUserConnect(Context.ConnectionId, user);

                return true;
            }
            catch (Exception e) 
            {
                _logger.LogDebug("User auth failed");
                _logger.LogDebug(e.ToString());
            }

            return false;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            _globalService.OnUserDisconnect(Context.ConnectionId);
        }

    }
}
