using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;


namespace WindTalkerMessenger.Services
{
    public class UserNameService : IUserNameService
    {
        private readonly IHttpContextAccessor _http;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserNameService(IHttpContextAccessor http, 
                          OnlineUsersLists onlineUsersLists,
                          UserManager<ApplicationUser> userManager,
                          ApplicationDbContext context)
        {
            _http = http;
            _onlineUsersLists = onlineUsersLists;
            _userManager = userManager;
            _context = context;
        }

        public bool IsUserNameAvailable(string guestName)
        {
            bool isAvailable;

            if (!_onlineUsersLists.anonUsers.Contains(guestName))
            {
                isAvailable = true;
            }
            else
            {
                isAvailable = false;
            }

            return isAvailable;
        }
        
        //Rename this method to be more concise regarding case of no identityemail
        public string GetIdentityChatName(string senderConnectionId)
        {
            
            var identityEmail = _http.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var chatNameKey = "";
            var senderChatName = "";
            
            if (identityEmail == null)
            {
                Guest guest = new Guest();
                //Need to grab the key from the connectionID value
                //senderChatName = (_onlineUsersLists.onlineUsers[chatNameKey] = senderConnectionId).ToString();
                guest = (Guest)_context.Guests.Where(x => x.GuestConnectionId == senderConnectionId);

            }
            else
            {
                senderChatName = _userManager.Users.Where(e => e.Email == identityEmail).Select(x => x.ChatName).ToString();
            }

            //return identityChatName;
            return senderChatName;
        }
    }
}
