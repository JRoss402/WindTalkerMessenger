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
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<HeartBeat> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string chatNameKey = "chatName";

        public HeartBeat(IContextService contextServices,
                       OnlineUsersLists onlineUsersLists,
                       IHttpContextAccessor httpContextAccessor,
                       ILogger<HeartBeat> logger,
                       UserManager<ApplicationUser> userManager)
        {
            _contextServices = contextServices;
            _onlineUsersLists = onlineUsersLists;
            _contextAccessor = httpContextAccessor;
            _logger = logger;
            _userManager = userManager;

        }

		public void NodesCheck()
		{
			var nodesCollection = _onlineUsersLists.clientHeartBeatCollection.ToList();
			foreach (ClientNode node in nodesCollection)
			{
				if ((node.LeftNodeReceive != node.RightNodeSend) && ((DateTime.Now - node.newTime) > TimeSpan.FromSeconds(6)))
				{
					_logger.LogInformation(node.NodeName.ToString() + ": Was disconnected due to no activity.");
					CleanUpClient(node);
				}
			}
		}

		public void CleanUpClient(ClientNode node)
		{
			try
			{
				string connectionId = node.NodeName;
				string identityEmail = _contextAccessor.HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Email)?.Value;
				var identityUser = _userManager.Users.First(x => x.UserName == identityEmail);
				string identityChatName = identityUser.ChatName;
				string chatName = node.chatName;
				bool isGuest = _onlineUsersLists.anonUsers.TryGetValue(chatName, out _);
				if (chatName == null)
				{
					chatName = identityChatName;
				}
				if (isGuest)
				{
					_contextServices.DisassociateGuestUserMessages(chatName);
				}
				_onlineUsersLists.onlineUsers.TryRemove(chatName, out _);
				_onlineUsersLists.anonUsers.TryRemove(chatName, out _);
				_onlineUsersLists.clientHeartBeatCollection.Remove(node);
				_onlineUsersLists.userLoginState.TryRemove(chatName, out _);
				_onlineUsersLists.authenticatedUsers.TryRemove(chatName, out _);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Issue cleaning up the client node {ex}");
			}
		}

		public void AddClientNode(string connectionId, string chatName)
		{
			ClientNode clientNode = new ClientNode();
			clientNode.NodeName = connectionId;
			clientNode.LeftNodeReceive = 0;
			clientNode.RightNodeSend = 0;
			clientNode.chatName = chatName;
			_onlineUsersLists.clientHeartBeatCollection.AddLast(clientNode);

		}

		public void HeartBeatAdd(string connectionId)
		{
			ClientNode node = new ClientNode();

			node = _onlineUsersLists.clientHeartBeatCollection.Where(u => u.NodeName == connectionId).First();
			node.LeftNodeReceive = node.RightNodeSend;
			node.RightNodeSend += 1;
		}

		public void NodeUpdate(string connectionId)
		{
			ClientNode node = new ClientNode();

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
			catch (Exception e)
			{
				_logger.LogError($"No node was found with that connection id.{e}");
			}


		}

	}
}
