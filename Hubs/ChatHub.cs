using WindTalkerMessenger.Services;
using Microsoft.AspNetCore.SignalR;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IContextService _contextServices;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserNameService _userNameService;
        private const string userNameKey = "userName";

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

        public async Task SendMessage(string receiverUserName, string message)
        {
            string senderConnectionId = Context.ConnectionId;
            string senderUserName = Context.User.Identity.Name;
            string receiverConnectionId = _onlineUsersLists.onlineUsers[receiverUserName].ToString();

            if(senderUserName == null)
            {
                senderUserName = _contextAccessor.HttpContext.Session.GetString(userNameKey);
            }

            var messageUID = Guid.NewGuid().ToString();
            _contextServices.CreateMessageObject(message, receiverUserName, senderUserName, messageUID, Status.Sent);

            if (_onlineUsersLists.onlineUsers.Contains(receiverUserName))
            {
                Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderUserName, message);
            }
            else
            {
                await Clients.Client(receiverConnectionId).SendAsync("MessageQueued", receiverUserName);
            }
        }

        public override async Task<Task> OnConnectedAsync()
        {
            string userName;
            string identityUserName = Context.User.Identity.Name;
            //string guestName = _contextAccessor.HttpContext.Session.GetString("guestName");
            //string identityUserName = _services.GradIdentityUserName();

            string connectionId = Context.ConnectionId.ToString();

            if (identityUserName != null)
            {
                userName = _userNameService.GetIdentityUserName();

            }
            else
            {
                userName = _contextAccessor.HttpContext.Session.GetString("guestName");

                _onlineUsersLists.anonUsers.Add(userName, connectionId);
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
