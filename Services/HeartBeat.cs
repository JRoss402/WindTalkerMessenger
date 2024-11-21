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

        public async Task TreeCheck()
        {
            var tree =  _onlineUsersLists.clientHeartBeatTree.ToList();
            foreach (ClientNode node in tree)
            {
                if ((node.LeftNodeReceive != node.RightNodeSend) && ((DateTime.Now - node.newTime) > TimeSpan.FromSeconds(6)))
                {
                    _logger.LogInformation(node.NodeName.ToString() + ": Was disconnected due to no activity.");
                    CleanUpClient(node);
                }
            }
        }

        public async void CleanUpClient(ClientNode node)
        {
            string connectionId = node.NodeName;
			string identityEmail = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var identityUser = _userManager.Users.First(x => x.UserName == identityEmail);
            string identityChatName = identityUser.ChatName;
            string chatName = _contextAccessor.HttpContext.Session.GetString(chatNameKey);
            if(chatName == null)
            {
                chatName = identityChatName;
            }
            if (_onlineUsersLists.anonUsers.TryGetValue(connectionId, out _))
            {
                _contextServices.DisassociateGuestUserMessages(chatName);
            }
            _onlineUsersLists.onlineUsers.TryRemove(chatName, out _);
            _onlineUsersLists.anonUsers.TryRemove(chatName, out _);
            _onlineUsersLists.clientHeartBeatTree.Remove(node);
            _onlineUsersLists.userLoginState.TryRemove(chatName, out _);
            _onlineUsersLists.authenticatedUsers.TryRemove(chatName, out _);
        }

        public async Task AddClientNode(string connectionId)
        {
            ClientNode clientNode = new ClientNode();
            clientNode.NodeName = connectionId;
            clientNode.LeftNodeReceive = 0;
            clientNode.RightNodeSend = 0;
            _onlineUsersLists.clientHeartBeatTree.AddLast(clientNode);

        }

        public async Task HeartBeatAdd(string connectionId)
        {
            ClientNode node = new ClientNode();

            node = _onlineUsersLists.clientHeartBeatTree.Where(u => u.NodeName == connectionId).First();
            node.LeftNodeReceive = node.RightNodeSend;
            node.RightNodeSend += 1;
        }

        public async Task NodeUpdate(string connectionId)
        {
            ClientNode node = new ClientNode();
            
            node = _onlineUsersLists.clientHeartBeatTree.Where(u => u.NodeName == connectionId).FirstOrDefault();
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
            catch(Exception e)
            {
                _logger.LogInformation("No node was found with that connection id.");
            }


        }

    }
}
