using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Controllers
{
    [Authorize]
    public class APIController : Controller
    {
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly IContextService _contextService;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<ApplicationUser> _userManager;

        public APIController(OnlineUsersLists onlineUsersLists,
                             IContextService contextService,
                             ApplicationDbContext context,
                             IHttpContextAccessor http,
                             UserManager<ApplicationUser> userManager) 
        {
            _onlineUsersLists = onlineUsersLists;
            _contextService = contextService;
            _context = context;
            _http = http;
            _userManager = userManager;
        }

        public bool CheckChatName(string chatName)
        {
            var isTaken = _onlineUsersLists.onlineUsers.ContainsKey(chatName);
            //bool json = true;

            if (isTaken == false)
            {
                return false; ;
            }

            return true;
        }

		public async Task<List<Message>> GetReceivedMessages(string chatName)
		{
            var userInfo = await _userManager.GetUserAsync(User);
            //var claims =await  _userManager.GetClaimsAsync(userInfo);
            var connectedUserChatName = userInfo.ChatName;
            //var receiverChatName = claims.FirstOrDefault(u => u.Type == "ReceiverChatName")?.Value;
            var chats = await _context.Chats
            				.Where((u =>( u.SenderChatName == chatName && u.ReceiverChatName == connectedUserChatName) ||
                                   ( u.SenderChatName == connectedUserChatName && u.ReceiverChatName == chatName))
                            ).ToListAsync();

            /*var query = from chat in _context.Chats
                        where (chat.SenderChatName == chatName &&
                        chat.ReceiverChatName == connectedUserChatName) &&
                        (chat.ReceiverChatName == connectedUserChatName &&
                         chat.SenderChatName == chatName)
                         select chat;*/

            //var json = JsonSerializer.Serialize(chats);
            //var chats = query.ToList();

            return chats;
		}
		/*public async Task<List<Message>> GetChats(string chatName)
		{
			var chats = await _contextService.GetReceivedMessages(chatName);

			return chats;
		}*/
	}
}
