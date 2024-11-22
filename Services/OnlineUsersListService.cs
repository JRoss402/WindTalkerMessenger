using System.Collections.Concurrent;
using WindTalkerMessenger.Models;

namespace WindTalkerMessenger.Services
{
    public class OnlineUsersLists
    {
        public ConcurrentDictionary<string,string> onlineUsers = new ConcurrentDictionary<string,string>();

        public ConcurrentDictionary<string,string> anonUsers = new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string,string> authenticatedUsers = new ConcurrentDictionary<string,string>();

        public ConcurrentDictionary<string,string> userLoginState = new ConcurrentDictionary<string,string>();

        public LinkedList<ClientNode> clientHeartBeatCollection = new LinkedList<ClientNode>();
    }
}
