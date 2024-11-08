﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

			var senderQueued = await _context.Queues.Where(u => u.SenderChatName == connectedUserChatName && u.ReceiverChatName == chatName).ToListAsync();

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
			var userInfo = await _userManager.GetUserAsync(User);
			var connectedUserChatName = userInfo.ChatName;

			HashSet<string> userList = new HashSet<string>();
            var senders = _context.Chats.Where(u => u.SenderChatName == connectedUserChatName).ToList();
            var receivers = _context.Chats.Where(u => u.ReceiverChatName == connectedUserChatName).ToList();
            //var chatNames = await _context.Queues.Where(u => u.SenderChatName == connectedUserChatName ||
            //                                                u.ReceiverChatName == connectedUserChatName).ToListAsync();

            var queuedNames = await GetQueuedChatList(connectedUserChatName);

            foreach(var sender in senders)
            {
                if (sender.ReceiverChatName != "User Account Deleted" && sender.ReceiverChatName != "Guest Disconnected")
                {
                    userList.Add(sender.ReceiverChatName.ToString());
                }

            }
            foreach(var receiver in receivers)
            {
                if (receiver.SenderChatName != "User Account Deleted" && receiver.SenderChatName != "Guest Disconnected")
                {
                    userList.Add(receiver.SenderChatName.ToString());

                }
            }
            foreach(string name in queuedNames)
            {
              userList.Add(name);
            }

            return userList;
        }

        public async Task<List<string>> GetQueuedChatList(string connectedUserChatName)
        {
            List<string> userList = new List<string>();

            var senders = _context.Queues.Where(u => u.SenderChatName == connectedUserChatName).ToList();
            var receivers = _context.Queues.Where(u => u.ReceiverChatName == connectedUserChatName).ToList();

            foreach (var sender in senders)
            {
                if(sender.ReceiverChatName != "User Account Deleted" && sender.ReceiverChatName != "Guest Disconnected")
                {
                    userList.Add(sender.ReceiverChatName.ToString());
                }
            }
            foreach (var receiver in receivers)
            {
                if (receiver.SenderChatName != "User Account Deleted" && receiver.SenderChatName != "Guest Disconnected")
                {
                    userList.Add(receiver.SenderChatName.ToString());

                }
            }

            return userList;
        }
        
	}
}
