using Microsoft.AspNetCore.Mvc;
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
		public MessagingController(OnlineUsersLists onlineUsersLists,
                                   IHttpContextAccessor httpAccessor,
								   IContextService contextService)
        {
			_onlineUsersLists = onlineUsersLists;
			_httpAccessor = httpAccessor;
			_contextService = contextService;
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
