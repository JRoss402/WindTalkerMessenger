using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;
using System.Web.Mvc;
using WindTalkerMessenger.Data.Migrations;
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
        private readonly IContextService _contextService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserNameService(IHttpContextAccessor http, 
                          OnlineUsersLists onlineUsersLists,
                          UserManager<ApplicationUser> userManager,
                          ApplicationDbContext context,
                          IContextService contextService,
                          SignInManager<ApplicationUser> signinmanager)
        {
            _http = http;
            _onlineUsersLists = onlineUsersLists;
            _userManager = userManager;
            _context = context;
            _contextService = contextService;
            _signInManager = signinmanager;
        }

        public List<string> GetAllUserNames()
        {
            var registeredUserNames =  _userManager.Users.Select(u => u.ChatName).ToList();
            
            return registeredUserNames;
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

        public async Task KillSwitchAsync(string userName)
        {
            var user =  await _userManager.FindByEmailAsync(userName);
            string userEmail = user.ToString();
            var result = await _userManager.DeleteAsync(user);

            _contextService.DisassociateIdentityUserMessages(userEmail);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }
            await _signInManager.SignOutAsync();

        }

        public async Task<bool> RegisterCheck(string username)
        {
            var regUsers = await _userManager.Users.ToListAsync();
            var names =  regUsers.Select(u => u.ChatName).ToList();
            if (names.Contains(username)){
                return true;
            }
            return false;
        }

    }
}
