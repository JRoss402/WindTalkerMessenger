using WindTalkerMessenger.Services;
using Microsoft.AspNetCore.SignalR;

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
			string senderConnectionId = Context.ConnectionId;
			string senderIdentityEmail = Context.User.Identity.Name;
			string senderChatName = _userNameService.GetSenderChatName(senderConnectionId);
			string? receiverConnectionId;
			_onlineUsersLists.onlineUsers.TryGetValue(receiverChatName, out receiverConnectionId);
			string receiverIdentityEmail = _userNameService.GetReceiverIdentityEmail(receiverChatName);
			bool isReceiverOnline = _onlineUsersLists.onlineUsers.ContainsKey(receiverChatName);
			bool isSenderRegistered = await _userNameService.IsUserRegistered(senderChatName);
			bool isReceiverRegistered = await _userNameService.IsUserRegistered(receiverChatName);

			//https://medium.com/programming-with-c/12-very-useful-shortcuts-in-c-programming-4b60242cedfa
			receiverConnectionId ??= "";

			if (isReceiverOnline)
			{
				_contextService.CreateMessageObject(message,
													 senderIdentityEmail,
													 receiverIdentityEmail,
													 Status.Sent,
													 senderChatName,
													 receiverChatName);

				await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderChatName, message);
			}
			else
			{
				if (isReceiverRegistered)
				{
					if (isSenderRegistered)
					{
						_contextService.CreateQueuedMessageObject(message,
																   senderIdentityEmail,
																   receiverIdentityEmail,
																   Status.Queued,
																   senderChatName,
																   receiverChatName);

					}
					await Clients.Client(senderConnectionId).SendAsync("MessageQueued", receiverChatName, message);
				}
				else
				{
					await Clients.Client(senderConnectionId).SendAsync("NoOne", receiverChatName, message);
				}
			}
		}

		public async Task HeartBeatResponse()
		{
			var connectionId = Context.ConnectionId;
			var userName = _userNameService.GetSenderChatName(connectionId);


			_logger.LogInformation("Pulse Received from {connectionId} ChatName: {userName}", connectionId, userName);
			_heartBeat.NodeUpdate(connectionId);
			await _heartBeat.NodesCheckAsync();
		}

		public override async Task<Task> OnConnectedAsync()
		{
			string connectionId = Context.ConnectionId;
			string chatName = _userNameService.GetSenderChatName(connectionId);
			bool isNewUserRegistered = Context.User.Identity.Name != null ? true : false;

			if (isNewUserRegistered)
			{
				try
				{
					_heartBeat.AddClientNode(connectionId, chatName);
					_onlineUsersLists.authenticatedUsers.TryAdd(chatName, connectionId);
					var newMessages = await _contextService.AddQueuedMessages(chatName);
					_onlineUsersLists.onlineUsers.TryAdd(chatName, connectionId);
					_onlineUsersLists.userLoginState.TryAdd(chatName, "Registered");

					await Clients.Users(connectionId).SendAsync("PrintQueuedMessages", newMessages);
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
					chatName = _http.HttpContext.Session.GetString(chatNameKey);
					_heartBeat.AddClientNode(connectionId, chatName);
					_contextService.AddNewGuest(chatName, connectionId);
					_onlineUsersLists.anonUsers.TryAdd(chatName, connectionId);
					_onlineUsersLists.onlineUsers.TryAdd(chatName, connectionId);
					_onlineUsersLists.userLoginState.TryAdd(chatName, "Guest");
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
			string connectionId = Context.ConnectionId.ToString();
			string chatName = _userNameService.GetSenderChatName(connectionId);
			string userType;
			_onlineUsersLists.userLoginState.TryGetValue(chatName, out userType);
			bool isGuest = userType == "Guest"; 

			if (isGuest)
			{
				chatName = _http.HttpContext.Session.GetString(chatNameKey);
				await _contextService.UpdateGuestMessagesAsync(chatName);
				_onlineUsersLists.anonUsers.TryRemove(chatName, out _);
			}
			_onlineUsersLists.onlineUsers.TryRemove(chatName, out _);
			_onlineUsersLists.authenticatedUsers.TryRemove(chatName, out _);
			_onlineUsersLists.userLoginState.TryRemove(chatName, out _);


			await base.OnDisconnectedAsync(exception);
		}

	}
}
