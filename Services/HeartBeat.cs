using System.Data.OleDb;
using WindTalkerMessenger.Hubs;
using WindTalkerMessenger.Models;

namespace WindTalkerMessenger.Services
{
    public class HeartBeat
    {

        private  readonly IContextService _contextServices;
        private  readonly OnlineUsersLists _onlineUsersLists;
        private  readonly IHttpContextAccessor _contextAccessor;
        private  readonly IUserNameService _userNameService;
        private  readonly ILogger _logger;

        public HeartBeat(IContextService contextServices,
                       OnlineUsersLists onlineUsersLists,
                       IHttpContextAccessor httpContextAccessor,
                       IUserNameService userNameService,
                       ILogger<ChatHub> logger)
        {
            _contextServices = contextServices;
            _onlineUsersLists = onlineUsersLists;
            _contextAccessor = httpContextAccessor;
            _userNameService = userNameService;
            _logger = logger;
        }

        public async Task TreeCheck()
        {
            var tree =  _onlineUsersLists.clientHeartBeatTree.ToList();
            foreach (ClientNode node in tree)
            {
                //(node.TimeSpread() > TimeSpan.FromSeconds(5)) <= span is too small to even register.

                if ((node.LeftNodeReceive != node.RightNodeSend) && ((DateTime.Now - node.newTime) > TimeSpan.FromSeconds(6)))
                //if((DateTime.Now - node.newTime) > TimeSpan.FromSeconds(6))
                {
                    _logger.LogInformation(node.NodeName.ToString() + ": Was disconnected due to no activity.");
                    CleanUpClient(node);
                }
            }
        }

        public void CleanUpClient(ClientNode node)
        {
            var connectionId = node.NodeName.ToString();
            string userName = _userNameService.GetSenderChatName(connectionId);

            _onlineUsersLists.onlineUsers.TryRemove(userName, out _);
            _onlineUsersLists.anonUsers.TryRemove(userName, out _);
            _onlineUsersLists.clientHeartBeatTree.Remove(node);
            _onlineUsersLists.userLoginState.TryRemove(userName, out _);
            _onlineUsersLists.authenticatedUsers.TryRemove(userName, out _);
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
            //node.oldTime = node.newTime;
            //node.newTime = DateTime.Now;
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
