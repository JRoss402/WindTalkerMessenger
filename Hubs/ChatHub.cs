﻿using WindTalkerMessenger.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using WindTalkerMessenger.Models.DomainModels;
using WindTalkerMessenger.Models;

namespace WindTalkerMessenger.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IContextService _contextServices;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserNameService _userNameService;
        private readonly HeartBeat _heartBeat;
        private const string chatNameKey = "chatName";
        private readonly ILogger<ChatHub> _logger;

        

        enum Status
        {
            Sent, Queued
        }

        public ChatHub(IContextService contextServices, 
                       OnlineUsersLists onlineUsersLists, 
                       IHttpContextAccessor httpContextAccessor,
                       IUserNameService userNameService,
                       ILogger<ChatHub> logger,
                       HeartBeat heartBeat)
        {
            _contextServices = contextServices;
            _onlineUsersLists = onlineUsersLists;
            _contextAccessor = httpContextAccessor;
            _userNameService = userNameService;
            _logger = logger;
            _heartBeat = heartBeat;
        }

        public async Task SendMessage(string receiverChatName, string message)
        {
            //Move most of the logic here to another class for object info gather methods
            string senderConnectionId = Context.ConnectionId;
            string senderIdentityEmail = Context.User.Identity.Name;
            string senderChatName = _userNameService.GetSenderChatName(senderConnectionId);
            string receiverConnectionId;


            if (!_onlineUsersLists.onlineUsers.TryGetValue(receiverChatName, out _))
            {
                receiverConnectionId = "";
            }
            else
            {
			    _onlineUsersLists.onlineUsers.TryGetValue(receiverChatName, out receiverConnectionId).ToString();
			}


            string receiverEmail = _userNameService.GetReceiverEmail(receiverChatName);
          

            if(receiverEmail == "")
            {
                receiverEmail = null;
            }
            string messageFamilyUID = Guid.NewGuid().ToString();


            if (_onlineUsersLists.onlineUsers.ContainsKey(receiverChatName))
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
                if(await _userNameService.RegisterCheck(receiverChatName) == true)
                {
                    if (_contextServices.IsUserGuest(senderChatName))
                    {
                        await Clients.Client(senderConnectionId).SendAsync("MessageQueued", receiverChatName, message);
                    }
                    else
                    {
                        _contextServices.CreateQueuedMessageObject(message,
                                                                   senderIdentityEmail,
                                                                   receiverEmail,
                                                                   messageFamilyUID,
                                                                   Status.Queued,
                                                                   senderChatName,
                                                                   receiverChatName);

                        await Clients.Client(senderConnectionId).SendAsync("MessageQueued", receiverChatName, message);
                    }
                }
                else
                {
                    await Clients.Client(senderConnectionId).SendAsync("NoOne", receiverChatName, message);

                }

            }
        }

        public async Task HeartBeatResponseAsync()
        {
            var connectionId = Context.ConnectionId;
            var userName = _userNameService.GetSenderChatName(connectionId);

            _logger.LogInformation("Pulse Received from " + connectionId + "ChatName: " + userName);
            await _heartBeat.NodeUpdate(connectionId);
            await _heartBeat.TreeCheck();
        }

        public override async Task<Task> OnConnectedAsync()
        {
            string userName;
            string identityUserName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId.ToString();
            try
            {
                await _heartBeat.AddClientNode(connectionId);
            }catch(Exception ex)
            {
                _logger.LogError($"Could not add the client node: {ex}");
            }

            if (identityUserName != null)
            {
                try
                {
                    userName = _userNameService.GetSenderChatName(connectionId);
                    _onlineUsersLists.authenticatedUsers.TryAdd(userName, connectionId);
                    var newMessages = await _contextServices.AddQueuedMessages(userName);
                    _onlineUsersLists.onlineUsers.TryAdd(userName, connectionId);
                    _onlineUsersLists.authenticatedUsers.TryAdd(userName, connectionId);
                    _onlineUsersLists.userLoginState.TryAdd(userName, "Registered");

                    await Clients.Users(connectionId).SendAsync("PrintQueuedMessages", newMessages);
                }catch(Exception ex)
                {
                    _logger.LogError($"Could not update User Login States: {ex}");
                }
			}
			else
            {
                try
                {
                    userName = _contextAccessor.HttpContext.Session.GetString(chatNameKey);
                    _contextServices.AddNewGuest(userName, connectionId);
                    _onlineUsersLists.anonUsers.TryAdd(userName, connectionId);
                    _onlineUsersLists.onlineUsers.TryAdd(userName, connectionId);
                    _onlineUsersLists.userLoginState.TryAdd(userName, "Guest");
                }catch(Exception ex)
                {
                    _logger.LogError($"Failed to process guest user: {ex}");
                }

            }

            return base.OnConnectedAsync();
        }

        

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string identityUserName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId.ToString();            
            string userName = _userNameService.GetSenderChatName(connectionId);

            if (userName == null)
            {
				userName = _contextAccessor.HttpContext.Session.GetString(chatNameKey);
				_contextServices.DisassociateGuestUserMessages(userName);
				_onlineUsersLists.anonUsers.TryRemove(userName, out _);
			}
            bool removed = _onlineUsersLists.onlineUsers.TryRemove(userName, out _);
            bool removedtwo = _onlineUsersLists.authenticatedUsers.TryRemove(userName, out _);
            bool gone = _onlineUsersLists.userLoginState.TryRemove(userName, out _);
            

            await base.OnDisconnectedAsync(exception);
        }

    }
}
