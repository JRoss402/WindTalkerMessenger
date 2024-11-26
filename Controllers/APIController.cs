using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Controllers
{
    //[Authorize]
    public class APIController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContextService _contextService;

		public APIController(ApplicationDbContext context,
                             UserManager<ApplicationUser> userManager,
                             IContextService contextService)
        {
            _context = context;
            _userManager = userManager;
            _contextService = contextService;
        }

		[HttpPost]
		[Route("API/GetReceivedMessages/{chatName}")]
		public async Task<List<Message>> GetReceivedMessages(string chatName)
		
        {
            var userInfo = await _userManager.GetUserAsync(User);
            var connectedUserChatName = userInfo.ChatName;
            var chats = await _context.Chats
            		        .Where((u =>( u.SenderChatName == chatName 
                                       && u.ReceiverChatName == connectedUserChatName)
                                     || ( u.SenderChatName == connectedUserChatName 
                                       && u.ReceiverChatName == chatName))).ToListAsync();
            
            return chats;
		}


        [HttpPost]
        [Route("API/GetSenderQueues/{chatName}")]
        public async Task<List<MessageQueue>> GetSenderQueues(string chatName)
        {
			var userInfo = await _userManager.GetUserAsync(User);
			var connectedUserChatName = userInfo.ChatName;

			var senderQueued = await _context.Queues.Where(u => u.SenderChatName == connectedUserChatName && u.ReceiverChatName == chatName).AsNoTracking().ToListAsync();

            return senderQueued;
        }

		public async Task<JsonResult> GetChatName()
        {
			var userInfo = await _userManager.GetUserAsync(User);
			var connectedUserChatName = userInfo.ChatName;

            return Json(connectedUserChatName);
		}

		public async Task<HashSet<string>> GetUserChatList()
        {
            var userList = await _contextService.GetChatNames();

            return userList;

        }


        
	}
}
