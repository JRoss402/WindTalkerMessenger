using WindTalkerMessenger.Services;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol;

namespace WindTalkerMessenger.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IContextService _contextServices;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserNameService _userNameService;
        private const string chatNameKey = "chatName";

        /* The transmission states. These will update during the entire process. The status can 
         * help with diagnosing any issues as well as recovering queued messages in the event
         * that the receiver is not online to receive them in real-time
         */

        enum Status
        {
            Sent, Received, Queued
        }

        public ChatHub(IContextService contextServices, 
                       OnlineUsersLists onlineUsersLists, 
                       IHttpContextAccessor httpContextAccessor,
                       IUserNameService userNameService)
        {
            _contextServices = contextServices;
            _onlineUsersLists = onlineUsersLists;
            _contextAccessor = httpContextAccessor;
            _userNameService = userNameService;
        }

        public async Task SendMessage(string receiverChatName, string message)
        {
            string senderConnectionId = Context.ConnectionId;

            //What if it is a guest-to-guest chat?
            string senderIdentityEmail = Context.User.Identity.Name;
            //Need to rename this method => GetIdentityChatName()
            string senderChatName = _userNameService.GetIdentityChatName(senderConnectionId);

            string receiverConnectionId = _onlineUsersLists.onlineUsers[receiverChatName].ToString();
            string messageFamilyUID = Guid.NewGuid().ToString();

            if (senderChatName == null)
            {
                senderChatName = _contextAccessor.HttpContext.Session.GetString(chatNameKey);
            }

            if (_onlineUsersLists.onlineUsers.Contains(receiverChatName))
            {
                _contextServices.CreateMessageObject(message, senderIdentityEmail, messageFamilyUID,
                                                     Status.Sent, senderChatName, receiverChatName);

                Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderChatName, message);
            }
            else
            {
                _contextServices.CreateQueuedMessageObject(message, senderIdentityEmail, messageFamilyUID,
                                                           Status.Sent, senderChatName, receiverChatName);

                await Clients.Client(receiverConnectionId).SendAsync("MessageQueued", receiverChatName);
            }
        }

        public override async Task<Task> OnConnectedAsync()
        {
            string userName;
            string identityUserName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId.ToString();

            if (identityUserName != null)
            {

               userName = _userNameService.GetIdentityChatName(connectionId);

            }
            else
            {
                userName = _contextAccessor.HttpContext.Session.GetString(chatNameKey);
                _contextServices.AddNewGuest(userName, connectionId);
                //_onlineUsersLists.onlineUsers.Add(userName,connectionId);
                //_onlineUsersLists.anonUsers.Add(userName, connectionId);
            }
            _onlineUsersLists.onlineUsers.Add(userName, connectionId);

            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.User(Context.ConnectionId).SendAsync("ServerDisconnect");

            string connectionId = Context.ConnectionId.ToString();

			string userName = Context.User.Identity.Name;
            _onlineUsersLists.onlineUsers.Remove(userName);
            _onlineUsersLists.anonUsers.Remove(userName);

            await base.OnDisconnectedAsync(exception);
        }

    }
}
