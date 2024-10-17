﻿using WindTalkerMessenger.Services;
using Microsoft.AspNetCore.SignalR;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IContextService _services;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly IHttpContextAccessor _contextAccessor;

        private const string guestKey = "guestName";

        /* The transmission states. These will update during the entire process. The status can 
         * help with diagnosing any issues as well as recovering queued messages in the event
         * that the receiver is not online to receive them in real-time
         */

        enum Status
        {
            //Message passed through SendMessage method
            Sent,
            //Message was received in real-time
            Received,
            //Message has been read
            Read,
            //Message was placed in a message que
            Queued,
        }

        public ChatHub(IContextService services, OnlineUsersLists onlineUsersLists, IHttpContextAccessor httpContextAccessor)
        {
            _services = services;
            _onlineUsersLists = onlineUsersLists;
            _contextAccessor = httpContextAccessor;
        }

        public async Task SendMessage(string receiverUser, string message)
        {

            string senderConnId = Context.ConnectionId;
            string receiverConnId = _onlineUsersLists.onlineUsers[receiverUser].ToString();
            string senderUser;

            if(Context.User.Identity.Name != null)
            {
                senderUser = Context.User.Identity.Name;
            }
            else
            {
                senderUser = _contextAccessor.HttpContext.Session.GetString(guestKey);
            }
            var ChatUID = Guid.NewGuid().ToString();
            ChatMessage msg = new ChatMessage();
            _services.CreateChatObject(message, receiverUser, senderUser, ChatUID, Status.Sent);

            if (_onlineUsersLists.onlineUsers.Contains(receiverUser))
            {
                Clients.Client(receiverConnId).SendAsync("ReceiveMessage", senderUser, message);
            }
            else
            {
                await Clients.Client(senderConnId).SendAsync("MessageQueued", receiverUser);
            }
        }

        public override async Task<Task> OnConnectedAsync()
        {
            string name;
            //string guestName = _contextAccessor.HttpContext.Session.GetString("guestName");
            //string identityUserName = _services.GradIdentityUserName();

            string connid = Context.ConnectionId.ToString();

            if (Context.User.Identity.Name != null)
            {
                 name = _services.GradIdentityUserName();

            }
            else
            {
                name = _contextAccessor.HttpContext.Session.GetString("guestName");

                _onlineUsersLists.anonUsers.Add(name, connid);
            }

            _onlineUsersLists.onlineUsers.Add(name, connid);

            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.User(Context.ConnectionId).SendAsync("ServerDisconnect");

            string connectionId = Context.ConnectionId.ToString();

			string name = Context.User.Identity.Name;
            _onlineUsersLists.onlineUsers.Remove(name);
            _onlineUsersLists.anonUsers.Remove(name);

            await base.OnDisconnectedAsync(exception);
        }

    }
}