﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
		public MessagingController(OnlineUsersLists onlineUsersLists,
                                   IHttpContextAccessor httpAccessor,
								   IContextService contextService,
								   IUserNameService userNameService)
        {
			_onlineUsersLists = onlineUsersLists;
			_httpAccessor = httpAccessor;
			_contextService = contextService;
			_userNameService = userNameService;
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

		public void Test()
		{
			Console.WriteLine("Here");
		}



	}
}
