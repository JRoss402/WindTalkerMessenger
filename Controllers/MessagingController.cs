using EnigmaMessengerV1.Controllers;
using Microsoft.AspNetCore.Mvc;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Controllers
{
	public class MessagingController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly OnlineUsersLists _onlineUsersLists;
		private readonly IHttpContextAccessor _httpAccessor;
		private readonly IContextService _contextService;
		public MessagingController(ILogger<HomeController> logger, OnlineUsersLists onlineUsersLists,
					  IHttpContextAccessor httpAccessor, IContextService contextService)
		{
			_logger = logger;
			_onlineUsersLists = onlineUsersLists;
			_httpAccessor = httpAccessor;
			_contextService = contextService;
		}

		[HttpPost]
		public IActionResult CheckGuestName(string guestName)
		{

			bool isTaken = _contextService.CheckGuestName(guestName);
			//ViewData["GuestNameTaken"] = isTaken;

			if (isTaken == true)
			{
				ViewData["GuestMessage"] = "That Username is taken. Please choose a new one";
				bool json = true;
				//return View("ChatOptions");
				return Json(json);
			}
			else
			{
				//bool json = false;
				//return Json(json);
				return View("ChatPage");
			}
		}

		public IActionResult Guest(string guestName)
		{
			_httpAccessor.HttpContext.Session.SetString("guestName", guestName);

			return View("Views\\Messaging\\ChatPage.cshtml");
		}


		public IActionResult StartChatting()
		{
			return View("Views\\Messaging\\ChatOptions.cshtml");
		}

		public void Test()
		{
			Console.WriteLine("Here");
		}

	}
}
