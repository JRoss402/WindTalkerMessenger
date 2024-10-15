using System.Collections;

namespace WindTalkerMessenger.Services
{
    public class OnlineUsersLists
    {
        public Hashtable onlineUsers = new Hashtable();

        public Hashtable anonUsers = new Hashtable();

        public void OnlineUsersAdd(string userName,string connectionId)
        {
            onlineUsers.Add(userName, connectionId);
        }

        public void OnlineUsersRemove(string userName)
        {
            onlineUsers.Remove(userName);
        }

        public void AnonUsersAdd(string userName, string connectionId)
        {
            anonUsers.Add(userName, connectionId);
        }

        public void AnonUsersRemove(string userName)
        {
            anonUsers.Remove(userName);
        }

    }
}
