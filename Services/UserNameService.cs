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

        public bool IsUserAuthenticated()
        {
            var auth = _http.HttpContext.User.Identity.IsAuthenticated;
            return auth;
        }

        public bool IsUserNameAvailable(string guestName)
        {
            bool isAvailable;
            if (!_onlineUsersLists.anonUsers.ContainsKey(guestName))
            {
                isAvailable = true;
            }
            else
            {
                isAvailable = false;
            }
            return isAvailable;
        }
        public string GetReceiverEmail(string username)
        {
            
            var grab = _userManager.Users.FirstOrDefault(u => u.ChatName == username);
            if (grab != null)
            {
                var receiverEmail = grab.Email;
                return receiverEmail;
            }
            else
            {
                var receiverEmail = "";
                return receiverEmail;
            }
        }
        public string GetSenderChatName(string senderConnectionId)
        {
            var identityEmail =  _http.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var chatNameKey = "";
            var senderChatName = "";
            
            if (identityEmail == null)
            {
                var grab = _context.Guests.FirstOrDefault(u => u.GuestConnectionId == senderConnectionId);
                senderChatName = grab.GuestName;
            }
            else
            {
               var grab = _userManager.Users.FirstOrDefault(e => e.Email == identityEmail);
                senderChatName = grab.ChatName;
            }
            return senderChatName;
        }
    }
}
