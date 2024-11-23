using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Controllers
{
	public class MessagingController : Controller
	{
		private readonly OnlineUsersLists _onlineUsersLists;
		private readonly IHttpContextAccessor _http;
		private readonly IUserNameService _userNameService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<MessagingController> _logger;
		private const string chatNameKey = "chatName";

		public MessagingController(OnlineUsersLists onlineUsersLists,
                                   IHttpContextAccessor http,
								   IUserNameService userNameService,
								   UserManager<ApplicationUser> userManager,
								   ILogger<MessagingController> logger)
        {
			_onlineUsersLists = onlineUsersLists;
			_http = http;
			_userNameService = userNameService;
			_userManager = userManager;
			_logger = logger;
		}

		public IActionResult Guest(string chatName)
		{
			_http.HttpContext.Session.SetString("chatName", chatName);

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

		[HttpPost]
		[Route("/SetGuestChatName/ChatName/{chatName}")]
		public void SetGuestChatName(string chatName)
		{
			try 
			{
				_http.HttpContext.Session.SetString(chatNameKey, chatName);
			}catch(Exception ex)
			{
				_logger.LogError($"New Guest Chat Name Could Not Be Set: {ex}");
			}
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
			await _userNameService.KillSwitchAsync(userName);

			return Redirect(("~/"));
		}

        [HttpPost]
        [Route("/IsReceiverRegistered/{username}")]
        public async Task<bool> IsReceiverRegistered(string username)
        {
			var isRegistered = await _userNameService.IsUserRegistered(username);

			return isRegistered;

        }

		[HttpPost]
        [Route("/CheckChatName/CheckName/{chatName}")]
        public async Task<bool> CheckChatName(string chatName)
        {
			var registeredChatNames = _userNameService.GetAllUserNames();

			if((registeredChatNames.Contains(chatName)) || (_onlineUsersLists.anonUsers.TryGetValue(chatName,out _))) 
			{
				return true;
			}

            return false;
		}

    }
}
