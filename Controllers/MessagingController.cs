using Microsoft.AspNetCore.Mvc;
using WindTalkerMessenger.Models.ExtensionMethods;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Controllers
{
	public class MessagingController : Controller
	{
		private readonly OnlineUsersLists _onlineUsersLists;
		private readonly IHttpContextAccessor _httpAccessor;
		public MessagingController(OnlineUsersLists onlineUsersLists,
                                   IHttpContextAccessor httpAccessor)
        {
			_onlineUsersLists = onlineUsersLists;
			_httpAccessor = httpAccessor;
		}

        public IActionResult Guest(string chatName)
		{
			_httpAccessor.HttpContext.Session.SetString("chatName", chatName);

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
