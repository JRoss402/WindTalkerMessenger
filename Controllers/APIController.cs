using Microsoft.AspNetCore.Mvc;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Controllers
{
    public class APIController : Controller
    {
        private readonly OnlineUsersLists _onlineUsersLists;

        public APIController(OnlineUsersLists onlineUsersLists) 
        {
            _onlineUsersLists = onlineUsersLists;
        }

        public bool CheckChatName(string chatName)
        {
            var isTaken = _onlineUsersLists.onlineUsers.Contains(chatName);
            bool json = true;

            if (isTaken == false)
            {
                json = false;
            }

            return json;
        }
    }
}
