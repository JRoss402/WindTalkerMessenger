using WindTalkerMessenger.Models;
using WindTalkerMessenger.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EnigmaMessengerV1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IContextService _contextService;
        
        public HomeController(ILogger<HomeController> logger, OnlineUsersLists onlineUsersLists, 
                              IHttpContextAccessor httpAccessor, IContextService contextService)
        {
            _logger = logger;
            _onlineUsersLists = onlineUsersLists;
            _httpAccessor = httpAccessor;
            _contextService = contextService;
        }

        public IActionResult Index()
        {
           return View();
        }

        /*[HttpPost]
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

            return View("ChatPage");
        }*/

        public IActionResult Privacy()
        {
            return View();
        }

        /*public IActionResult StartChatting()
        {
            return View("ChatOptions");
        }*/

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
