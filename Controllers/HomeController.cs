using WindTalkerMessenger.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WindTalkerMessenger.Services;
using NuGet.Protocol;

namespace EnigmaMessengerV1.Controllers
{
    public class HomeController : Controller
    {
        private readonly OnlineUsersLists _onlineUsersLists;

        public HomeController(OnlineUsersLists onlineUsersLists)
        {
            _onlineUsersLists = onlineUsersLists;
        }


        public IActionResult Index()
        {
            
           return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
		public IActionResult About()
		{
			return View();
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [Route ("/{chatName}")]

        public JsonResult CheckChatName(string chatName)
		{
			var isTaken = _onlineUsersLists.onlineUsers.ContainsKey(chatName);

			return Json(isTaken);
		}
	}
}
