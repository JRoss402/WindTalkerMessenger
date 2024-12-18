﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WindTalkerMessenger.Models.DataLayer;


namespace WindTalkerMessenger.Services
{
    public class UserNameService : IUserNameService
    {
        private readonly IHttpContextAccessor _http;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContextService _contextService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<UserNameService> _logger;
		private const string chatNameKey = "chatName";

		public UserNameService(IHttpContextAccessor http, 
                          OnlineUsersLists onlineUsersLists,
                          UserManager<ApplicationUser> userManager,
                          IContextService contextService,
                          SignInManager<ApplicationUser> signinmanager,
                          ILogger<UserNameService> logger)
        {
            _http = http;
            _onlineUsersLists = onlineUsersLists;
            _userManager = userManager;
            _contextService = contextService;
            _signInManager = signinmanager;
            _logger = logger;
        }

        public async Task<List<string>> GetAllUserNames()
        {
            var registeredUserNames = await _userManager.Users.Select(u => u.ChatName).ToListAsync();
            
            return registeredUserNames;
        }

        public bool IsUserAuthenticated()
        {
            var auth = _http.HttpContext.User.Identity.IsAuthenticated;
            return auth;
        }

        public bool IsUserNameAvailable(string guestName)
        {
            bool isAvailable;
            if (!_onlineUsersLists.anonUsers.ContainsKey(guestName))
            {
                isAvailable = true;
            }
            else
            {
                isAvailable = false;
            }
            return isAvailable;
        }

		public string GetReceiverIdentityEmail(string username)
		{
			var grab = _userManager.Users.FirstOrDefault(u => u.ChatName == username);
			if (grab != null)
			{
				var receiverEmail = grab.Email;
				return receiverEmail;
			}
			else
			{
				return null;
			}
		}
		public string GetSenderChatName(string senderConnectionId)
        {
			var identityEmail = _http.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
			var senderChatName = "";
			try
            {

				if (identityEmail == null)
                {
                    senderChatName = _http.HttpContext.Session.GetString(chatNameKey);
                }
                else
                {
                    var grab = _userManager.Users.FirstOrDefault(e => e.Email == identityEmail);
                    senderChatName = grab.ChatName;
                }
            } catch(Exception ex)
            {
                _logger.LogError($"Error Getting Chat Name: {ex}");
            }
            return senderChatName;
        }

        public async Task KillSwitchAsync(string userName)
        {
            string chatName = GetSenderChatName("");
            await _contextService.RemoveRegisteredUserAsync(chatName);
            var user =  await _userManager.FindByEmailAsync(userName);
            string userEmail = user.ToString();
            var result = await _userManager.DeleteAsync(user);

            await _contextService.UpdateIdentityMessagesAsync(userEmail);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }
            await _signInManager.SignOutAsync();

        }

        public async Task<bool> IsUserRegistered(string username)
        {
            var regUsers = await _userManager.Users.ToListAsync();
            var names =  regUsers.Select(u => u.ChatName).ToList();
            if (names.Contains(username)){
                return true;
            }
            return false;
        }
    }
}
