using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WindTalkerMessenger.Models.DataLayer;

namespace WindTalkerMessenger.Services
{
    public class UserNameService : IUserNameService
    {
        private readonly IHttpContextAccessor _http;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserNameService(IHttpContextAccessor http, 
                          OnlineUsersLists onlineUsersLists,
                          UserManager<ApplicationUser> userManager)
        {
            _http = http;
            _onlineUsersLists = onlineUsersLists;
            _userManager = userManager;
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

        public string GetIdentityUserName()
        {
            var identityEmail = _http.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var identityUserName = _userManager.Users.Where(e => e.Email == identityEmail).Select(x => x.UserUserName).ToString();
            
            return identityUserName;
        }
    }
}
