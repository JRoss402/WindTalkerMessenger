using WindTalkerMessenger.Services;
using Microsoft.AspNetCore.SignalR;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Hubs
{
	public class ChatHub : Hub
	{
		private readonly IContextService _contextService;
		private readonly OnlineUsersLists _onlineUsersLists;
		private readonly IHttpContextAccessor _http;
		private readonly IUserNameService _userNameService;
		private readonly HeartBeat _heartBeat;
		private const string chatNameKey = "chatName";
		private readonly ILogger<ChatHub> _logger;

		enum Status
		{
			Sent, Queued
		}

		public ChatHub(IContextService contextService,
					   OnlineUsersLists onlineUsersLists,
					   IHttpContextAccessor http,
					   IUserNameService userNameService,
					   ILogger<ChatHub> logger,
					   HeartBeat heartBeat)
		{
			_contextService = contextService;
			_onlineUsersLists = onlineUsersLists;
			_http = http;
			_userNameService = userNameService;
			_logger = logger;
			_heartBeat = heartBeat;
		}

		public async Task SendMessage(string receiverChatName, string message)
		{
            string? receiverConnectionId;
            _onlineUsersLists.onlineUsers.TryGetValue(receiverChatName, out receiverConnectionId);

			Sender sender = new Sender();
			sender.ConnectionId = Context.ConnectionId;
			sender.ChatName = _userNameService.GetSenderChatName(sender.ConnectionId);
			sender.EMail = Context.User.Identity.Name;

			Receiver receiver = new Receiver();
			receiver.ConnectionId = receiverConnectionId;
			receiver.ChatName = receiverChatName;
			receiver.EMail = _userNameService.GetReceiverIdentityEmail(receiver.ChatName);

			bool isReceiverOnline = _onlineUsersLists.onlineUsers.ContainsKey(receiver.ChatName);
			bool isSenderRegistered = await _userNameService.IsUserRegistered(sender.ChatName);
			bool isReceiverRegistered = await _userNameService.IsUserRegistered(receiver.ChatName);

            if (isReceiverOnline)
			{
				_contextService.CreateMessageObject(message,
													sender.EMail,
													receiver.EMail,
													Status.Sent,
													sender.ChatName,
													receiver.ChatName);

				await Clients.Client(receiver.ConnectionId).SendAsync("ReceiveMessage", sender.ChatName, message);
			}
			else
			{
				if (isReceiverRegistered)
				{
					if (isSenderRegistered)
					{
						_contextService.CreateQueuedMessageObject(message,
																  sender.EMail,
                                                                  receiver.EMail,
																  Status.Queued,
																  sender.ChatName,
                                                                  receiver.ChatName);

					}
					await Clients.Client(sender.ConnectionId).SendAsync("MessageQueued", receiver.ChatName, message);
				}
				else
				{
					await Clients.Client(sender.ConnectionId).SendAsync("NoOne", receiver.ChatName, message);
				}
			}
		}

		public async Task HeartBeatResponse()
		{
			Client client = new Client();
			client.ConnectionId = Context.ConnectionId;
			client.ChatName = _userNameService.GetSenderChatName(client.ConnectionId);

			_logger.LogInformation("Pulse Received from {connectionId} ChatName: {userName}", client.ConnectionId, client.ChatName);
			_heartBeat.NodeUpdate(client.ConnectionId);
			await _heartBeat.NodesCheckAsync();
		}

		public override async Task<Task> OnConnectedAsync()
		{
			Client client = new Client();
			client.ConnectionId = Context.ConnectionId;
			client.ChatName = _userNameService.GetSenderChatName(client.ConnectionId);
			bool isNewUserRegistered = Context.User.Identity.Name != null ? true : false;

			if (isNewUserRegistered)
			{
				try
				{
                    var newMessages = await _contextService.AddQueuedMessages(client.ChatName);
                    _heartBeat.AddClientNode(client.ConnectionId, client.ChatName);
                    await _contextService.UpdateRegistredUsersAsync(client.ConnectionId, client.ChatName);
                    await Clients.Users(client.ConnectionId).SendAsync("PrintQueuedMessages", newMessages);
				}
				catch (Exception ex)
				{
					_logger.LogError("Could not update User Login States: {ex}", ex);
				}
			}
			else
			{
				try
				{
					client.ChatName = _http.HttpContext.Session.GetString(chatNameKey);
					_heartBeat.AddClientNode(client.ConnectionId, client.ChatName);
					_contextService.AddNewGuest(client.ChatName, client.ConnectionId);

					await _contextService.UpdateGuestUsersAsync(client.ConnectionId, client.ChatName);
				}
				catch (Exception ex)
				{
					_logger.LogError("Failed to process guest user: {ex}", ex);
				}

			}
			return base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			Client client = new Client();
			client.ConnectionId = Context.ConnectionId;
			client.ChatName = _userNameService.GetSenderChatName(client.ConnectionId);
			
			string userType;
			_onlineUsersLists.userLoginState.TryGetValue(client.ChatName, out userType);
			bool isGuest = userType == "Guest"; 

			if (isGuest)
			{
				client.ChatName = _http.HttpContext.Session.GetString(chatNameKey);
				await _contextService.UpdateGuestMessagesAsync(client.ChatName);
				await _contextService.RemoveGuestUserAsync(client.ChatName);
			}
			await _contextService.RemoveRegisteredUserAsync(client.ChatName);

			await base.OnDisconnectedAsync(exception);
		}

	}
}
