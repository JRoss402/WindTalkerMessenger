using System.Collections;
using System.Collections.Concurrent;
using System.Security.Policy;

namespace WindTalkerMessenger.Services
{
    public class OnlineUsersLists
    {
        public ConcurrentDictionary<string,string> onlineUsers = new ConcurrentDictionary<string,string>();

        public ConcurrentDictionary<string,string> anonUsers = new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string,string> authenticatedUsers = new ConcurrentDictionary<string,string>();

        public ConcurrentDictionary<string,string> userLoginState = new ConcurrentDictionary<string,string>();
        //Rename these functions and place the verb in front.
       /* public async Task OnlineUsersAdd(string userName,string connectionId)
        {
            onlineUsers.TryAdd(userName, connectionId);
        }

        public async Task OnlineUsersRemove(string userName,string connectionId)
        {
            onlineUsers.TryRemove(KeyValuePair<userName,connectionId);
        }

        public async Task AnonUsersAdd(string userName, string connectionId)
        {
            anonUsers.Add(userName, connectionId);
        }

        public async Task AnonUsersRemove(string userName)
        {
            anonUsers.Remove(userName);
        }*/

    }
}
