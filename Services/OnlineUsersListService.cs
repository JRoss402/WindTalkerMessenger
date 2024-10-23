using System.Collections;

namespace WindTalkerMessenger.Services
{
    public class OnlineUsersLists
    {
        public Hashtable onlineUsers = new Hashtable();

        public Hashtable anonUsers = new Hashtable();

        //Rename these functions and place the verb in front.
        public async Task OnlineUsersAdd(string userName,string connectionId)
        {
            onlineUsers.Add(userName, connectionId);
        }

        public async Task OnlineUsersRemove(string userName)
        {
            onlineUsers.Remove(userName);
        }

        public async Task AnonUsersAdd(string userName, string connectionId)
        {
            anonUsers.Add(userName, connectionId);
        }

        public async Task AnonUsersRemove(string userName)
        {
            anonUsers.Remove(userName);
        }

    }
}
