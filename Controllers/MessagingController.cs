using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;
using WindTalkerMessenger.Models.ExtensionMethods;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Controllers
{
	public class MessagingController : Controller
	{
		private readonly OnlineUsersLists _onlineUsersLists;
		private readonly IHttpContextAccessor _httpAccessor;
		private readonly IContextService _contextService;
		private readonly IUserNameService _userNameService;
		private readonly UserManager<ApplicationUser> _userManager;
		public MessagingController(OnlineUsersLists onlineUsersLists,
                                   IHttpContextAccessor httpAccessor,
								   IContextService contextService,
								   IUserNameService userNameService,
								   UserManager<ApplicationUser> userManager)
        {
			_onlineUsersLists = onlineUsersLists;
			_httpAccessor = httpAccessor;
			_contextService = contextService;
			_userNameService = userNameService;
			_userManager = userManager;
		}

        public IActionResult Guest(string chatName)
		{
			
			_httpAccessor.HttpContext.Session.SetString("chatName", chatName);

			return View("Views\\Messaging\\ChatPage.cshtml");
		}

        [HttpPost]
        [Route("/GetLoginState/{chatName}")]
        public bool GetLoginState(string chatName)
        {
			string userLoginState;
			_onlineUsersLists.userLoginState.TryGetValue(chatName, out userLoginState);
			bool state = true;

			if(userLoginState != null) 
			{ 
				if(userLoginState == "Guest")
				{
					state = false;
				}
			}

            return state;

        }

		public async Task<bool> GetReceiverRegistrationState(string receiverChatName)
		{
			bool state = true;
			var registeredUsers = await _userManager.Users.Select(u => u.UserName).ToListAsync();
			if (!registeredUsers.Contains(receiverChatName))
			{
				state = false;
			}

			return state;
		}

        public IActionResult StartChatting()
		{
			if (_userNameService.IsUserAuthenticated())
			{
				return View("Views\\Messaging\\ChatPage.cshtml");
			}
			else
			{
				return View("Views\\Messaging\\ChatOptions.cshtml");

			}
		}
		[HttpPost]
        public async Task<IActionResult> KillSwitchAsync(string userName)
		{
			//var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
			await _userNameService.KillSwitchAsync(userName);

			return Redirect(("~/"));
		}

        [HttpPost]
        [Route("/IsReceiverRegistered/{username}")]
        public async Task<bool> IsReceiverRegistered(string username)
        {
			var isRegistered = await _userNameService.RegistserCheck(username);

			return isRegistered;

        }

    }
}
