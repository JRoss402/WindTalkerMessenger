using WindTalkerMessenger.Services;
using Microsoft.AspNetCore.SignalR;

namespace WindTalkerMessenger.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IContextService _contextServices;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserNameService _userNameService;
        private const string chatNameKey = "chatName";

        /*
         * 1 - Grab a list of the user's current when they connect
         * 2 - Create a list of button with the chatnames via javascript
         * 3 - When the user clicks the button => load the chats
         * 4 - GetCurrentChats(string receiverChatName)
         * 5 - 
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
            //Need to change logic to make messageFamily the same for a chat sequence.
            /*
                If receiverName = recName AND senderName = sendName AND count = 1
             */

            string senderConnectionId = Context.ConnectionId;
            string senderIdentityEmail = Context.User.Identity.Name;
            string senderChatName = _userNameService.GetSenderChatName(senderConnectionId);
            string receiverConnectionId = _onlineUsersLists.onlineUsers[receiverChatName].ToString();
            string receiverEmail = _userNameService.GetReceiverEmail(receiverChatName);
            if(receiverEmail == "")
            {
                receiverEmail = null;
            }
            string messageFamilyUID = Guid.NewGuid().ToString();

            /*if (senderChatName == null)
            {
                senderChatName = _contextAccessor.HttpContext.Session.GetString(chatNameKey);
            }*/

            if (_onlineUsersLists.onlineUsers.Contains(receiverChatName))
            {
                _contextServices.CreateMessageObject(message, 
                                                     senderIdentityEmail,
                                                     receiverEmail,
                                                     messageFamilyUID,
                                                     Status.Sent, 
                                                     senderChatName, 
                                                     receiverChatName);

                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderChatName, message);
            }
            else
            {
                _contextServices.CreateQueuedMessageObject(message, 
                                                           senderIdentityEmail,
                                                           receiverEmail,
                                                           messageFamilyUID,
                                                           Status.Sent, 
                                                           senderChatName, 
                                                           receiverChatName);

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
               userName = _userNameService.GetSenderChatName(connectionId);
            }
            else
            {
                userName = _contextAccessor.HttpContext.Session.GetString(chatNameKey);
                _contextServices.AddNewGuest(userName, connectionId);
                //var currentChatNames = _contextServices.GetChatFriends(userName);
                //await Clients.Client(connectionId).SendAsync("ChatNamesList", currentChatNames);

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
