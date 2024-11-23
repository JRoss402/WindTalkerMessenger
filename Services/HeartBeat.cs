using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WindTalkerMessenger.Models;
using WindTalkerMessenger.Models.DataLayer;

namespace WindTalkerMessenger.Services
{
    public class HeartBeat
    {
        private readonly IContextService _contextServices;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<HeartBeat> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HeartBeat(IContextService contextServices,
                       OnlineUsersLists onlineUsersLists,
                       IHttpContextAccessor http,
                       ILogger<HeartBeat> logger,
                       UserManager<ApplicationUser> userManager)
        {
            _contextServices = contextServices;
            _onlineUsersLists = onlineUsersLists;
			_http = http;
            _logger = logger;
            _userManager = userManager;
        }

		public async Task NodesCheckAsync()
		{
			var nodesCollection = _onlineUsersLists.clientHeartBeatCollection.ToList();
			foreach (ClientNode node in nodesCollection)
			{
				if ((node.LeftNodeReceive != node.RightNodeSend) && ((DateTime.Now - node.newTime) > TimeSpan.FromSeconds(6)))
				{
					_logger.LogInformation("ConnectionID: {node.NodeName} - ChatName:{node.chatName} was disconnected due to no activity.",node.NodeName,node.chatName);
					await CleanUpClientAsync(node);
				}
			}
		}

		public async Task CleanUpClientAsync(ClientNode node)
		{
			try
			{
				string connectionId = node.NodeName;
				string identityEmail = _http?.HttpContext?.User.Claims.First(x => x.Type == ClaimTypes.Email)?.Value ?? "";
				var identityUser = _userManager.Users.First(x => x.UserName == identityEmail);
				string identityChatName = identityUser.ChatName;
				string? chatName = node.chatName ?? "";
				bool isGuest = _onlineUsersLists.anonUsers.TryGetValue(chatName, out _);
				if (chatName == null)
				{
					chatName = identityChatName;
				}
				if (isGuest)
				{
					await _contextServices.DisassociateGuestUserMessagesAsync(chatName);
				}
				_onlineUsersLists.onlineUsers.TryRemove(chatName, out _);
				_onlineUsersLists.anonUsers.TryRemove(chatName, out _);
				_onlineUsersLists.clientHeartBeatCollection.Remove(node);
				_onlineUsersLists.userLoginState.TryRemove(chatName, out _);
				_onlineUsersLists.authenticatedUsers.TryRemove(chatName, out _);
			}
			catch (Exception ex)
			{
				_logger.LogError("Issue cleaning up the client node {ex}",ex);
			}
		}

		public void AddClientNode(string connectionId, string chatName)
		{
			ClientNode clientNode = new()
			{
				NodeName = connectionId,
				LeftNodeReceive = 0,
				RightNodeSend = 0,
				chatName = chatName
			};
			_onlineUsersLists.clientHeartBeatCollection.AddLast(clientNode);
		}

		public void HeartBeatAdd(string connectionId)
		{
			ClientNode node = new();

			node = _onlineUsersLists.clientHeartBeatCollection.Where(u => u.NodeName == connectionId).First();
			node.LeftNodeReceive = node.RightNodeSend;
			node.RightNodeSend += 1;
		}

		public void NodeUpdate(string connectionId)
		{
			ClientNode node = new();

			node = _onlineUsersLists.clientHeartBeatCollection.Where(u => u.NodeName == connectionId).First();
			try
			{
				if (node != null)
				{
					var old = node.newTime;
					node.LeftNodeReceive += 1;
					node.oldTime = old;
					node.newTime = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("No node was found with that connection id.{e}",ex);
			}
		}

	}
}
