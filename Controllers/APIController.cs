using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Controllers
{
    public class APIController : Controller
    {
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly IContextService _contextService;
        private readonly ApplicationDbContext _context;

        public APIController(OnlineUsersLists onlineUsersLists,
                             IContextService contextService,
                             ApplicationDbContext context) 
        {
            _onlineUsersLists = onlineUsersLists;
            _contextService = contextService;
            _context = context;
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
			var chats = await _context.Chats
							.Where(u => u.ReceiverChatName == chatName).ToListAsync();

            //var json = JsonSerializer.Serialize(chats);

            return chats;
		}
		/*public async Task<List<Message>> GetChats(string chatName)
		{
			var chats = await _contextService.GetReceivedMessages(chatName);

			return chats;
		}*/
	}
}
