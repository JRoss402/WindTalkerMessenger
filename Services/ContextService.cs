using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WindTalkerMessenger.Hubs;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public class ContextService : IContextService
    {
        enum Status
        {
            Sent, Queued
        }

        private readonly ApplicationDbContext _context;
        private const string USER_DELETED = "User Account Deleted";
        private const string MSG_DELETED = "User Deleted Messages";
        private readonly ILogger<ContextService> _logger;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OnlineUsersLists _onlineUsersLists;
        private enum Statuses { Sent, Queued }

        public ContextService(ApplicationDbContext context,
                              ILogger<ContextService> logger,
                              IHttpContextAccessor http,
                              UserManager<ApplicationUser> userManager,
                              OnlineUsersLists onlineUsersLists)
        {
            _context = context;
            _logger = logger;
            _http = http;
            _userManager = userManager;
            _onlineUsersLists = onlineUsersLists;

        }


        public async Task UpdateRegistredUsersAsync(string connectionId, string chatName)
        {
            _onlineUsersLists.authenticatedUsers.TryAdd(chatName, connectionId);
            _onlineUsersLists.onlineUsers.TryAdd(chatName, connectionId);
            _onlineUsersLists.userLoginState.TryAdd(chatName, "Registered");
        }

        public async Task UpdateGuestUsersAsync(string connectionId, string chatName)
        {
            _onlineUsersLists.anonUsers.TryAdd(chatName, connectionId);
            _onlineUsersLists.onlineUsers.TryAdd(chatName, connectionId);
            _onlineUsersLists.userLoginState.TryAdd(chatName, "Guest");
        }

        public async Task RemoveRegisteredUserAsync(string chatName)
        {
            _onlineUsersLists.onlineUsers.TryRemove(chatName, out _);
            _onlineUsersLists.authenticatedUsers.TryRemove(chatName, out _);
            _onlineUsersLists.userLoginState.TryRemove(chatName, out _);
        }
        public async Task RemoveGuestUserAsync(string chatName)
        {
            _onlineUsersLists.anonUsers.TryRemove(chatName, out _);
            _onlineUsersLists.onlineUsers.TryRemove(chatName, out _);
            _onlineUsersLists.userLoginState.TryRemove(chatName, out _);
        }

        public async Task<HashSet<string>> GetChatNames()
        {
            HashSet<string> userList = new HashSet<string>();

            try
            {
                var User = _http.HttpContext.User;
                var userInfo = await _userManager.GetUserAsync(User);
                var connectedUserChatName = userInfo?.ChatName;
                connectedUserChatName ??= "";

#pragma warning disable CS8634
#pragma warning disable CS8620
                if (connectedUserChatName != "")
                {

                    var senders = await _context.Chats.Where(u => u.SenderChatName == connectedUserChatName
                                                               && u.ReceiverChatName != "User Account Deleted"
                                                               && u.ReceiverChatName != "Guest Disconnected")
                                                      .Select(n => n.ReceiverChatName)
                                                      .AsNoTracking()
                                                      .ToListAsync();



                    var receivers = await _context.Chats.Where(u => u.ReceiverChatName == connectedUserChatName
                                                                 && u.SenderChatName != "User Account Deleted"
                                                                 && u.SenderChatName != "Guest Disconnected")
                                                        .Select(n => n.SenderChatName)
                                                        .AsNoTracking()
                                                        .ToListAsync();
#pragma warning restore CS8620
#pragma warning restore CS8634
                    var queuedNames = await GetQueuedChatList(connectedUserChatName);

                    userList.UnionWith(senders);
                    userList.UnionWith(receivers);
                    userList.UnionWith(queuedNames);


                }
            }
            catch (Exception ex)
            {
                _logger.LogError("There was an error grabbing the chat name lists: {ex}", ex);
            }
            return userList;

        }
        public async Task<HashSet<string>> GetQueuedChatList(string connectedUserChatName)
        {
            HashSet<string> userList = new HashSet<string>();
            try
            {
#pragma warning disable CS8634
#pragma warning disable CS8620
                var senders = await _context.Queues.Where(u => u.SenderChatName == connectedUserChatName
                                                            && u.ReceiverChatName != "User Account Deleted"
                                                            && u.ReceiverChatName != "Guest Disconnected")
                                                   .Select(n => n.ReceiverChatName)
                                                   .AsNoTracking()
                                                   .ToListAsync();



                var receivers = await _context.Queues.Where(u => u.ReceiverChatName == connectedUserChatName
                                                              && u.SenderChatName != "User Account Deleted"
                                                              && u.SenderChatName != "Guest Disconnected")
                                                     .Select(n => n.SenderChatName)
                                                     .AsNoTracking()
                                                     .ToListAsync();
#pragma warning restore CS8620
#pragma warning restore CS8634

                userList.UnionWith(senders);
                userList.UnionWith(receivers);
            }
            catch (Exception ex)
            {
                _logger.LogError($"There was an error loading queued chat names {ex}", ex);
            }

            return userList;
        }

        public void AddNewGuest(string userName,
                                string connectionId)
        {
            Guest guest = new Guest();
            string guestuid = Guid.NewGuid().ToString();
            guest.GuestName = userName;
            guest.GuestConnectionId = connectionId;
            guest.GuestUID = guestuid;
            guest.AddedDate = DateTime.Now;
            try
            {
                _context.Guests.Add(guest);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {

                _logger.LogError("Guest Was Not Added: {ex}", ex);
            }
        }

        public async Task UpdateGuestMessagesAsync(string guestChatName)
        {
            var chats = _context.Chats.Where(u => u.SenderChatName == guestChatName ||
                                      u.ReceiverChatName == guestChatName).ToList();

            foreach (Message chat in chats)
            {
                if (chat.SenderChatName == guestChatName)
                {
                    chat.SenderChatName = "Guest Disconnected";
                    chat.UserMessage = "Guest Disconnected";
                    //IsRowRemovable(chat);

                }
                else if (chat.ReceiverChatName == guestChatName)
                {
                    chat.ReceiverChatName = "Guest Disconnected";
                    //IsRowRemovable(chat);

                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateIdentityMessagesAsync(string identityUserEmail)
        {
            var userMessages = _context.Chats.Where(e => e.MessageSenderEmail == identityUserEmail ||
                                                          e.MessageReceiverEmail == identityUserEmail).ToList();

            foreach (Message userMessage in userMessages)
            {
                if (userMessage.MessageSenderEmail == identityUserEmail)
                {
                    userMessage.MessageSenderEmail = USER_DELETED;
                    userMessage.SenderChatName = USER_DELETED;
                    userMessage.UserMessage = MSG_DELETED;
                }
                else if (userMessage.MessageReceiverEmail == identityUserEmail)
                {
                    userMessage.MessageReceiverEmail = USER_DELETED;
                    userMessage.ReceiverChatName = USER_DELETED;
                }

                IsMessageRowRemovable(userMessage);

                _context.Chats.Update(userMessage);
            }
            await _context.SaveChangesAsync();

            DeleteIdentityQueuedMessagesAsync(identityUserEmail);

        }

        public async Task DeleteIdentityQueuedMessagesAsync(string identityUserEmail)
        {
            var userQueuedMessages = await _context.Queues.Where(e => e.MessageSenderEmail == identityUserEmail ||
                                              e.MessageReceiverEmail == identityUserEmail).ToListAsync();

            foreach (MessageQueue userQueuedMessage in userQueuedMessages)
            {
                if (userQueuedMessage.MessageSenderEmail == identityUserEmail)
                {
                    userQueuedMessage.MessageSenderEmail = USER_DELETED;
                    userQueuedMessage.SenderChatName = USER_DELETED;
                    userQueuedMessage.UserMessage = MSG_DELETED;
                }
                else if (userQueuedMessage.MessageReceiverEmail == identityUserEmail)
                {
                    userQueuedMessage.MessageReceiverEmail = USER_DELETED;
                    userQueuedMessage.ReceiverChatName = USER_DELETED;
                }

                IsQueueRowRemovable(userQueuedMessage);

                _context.Queues.Update(userQueuedMessage);
            }
            await _context.SaveChangesAsync();

        }

        public void IsQueueRowRemovable(MessageQueue queue)
        {
            if (queue.MessageSenderEmail == USER_DELETED &&
                queue.MessageReceiverEmail == USER_DELETED)
            {
                _context.Queues.Remove(queue);
            }
        }

        public void IsMessageRowRemovable(Message message)
        {
            if (message.MessageSenderEmail == USER_DELETED &&
               message.MessageReceiverEmail == USER_DELETED)
            {
                _context.Chats.Remove(message);
            }
        }

        public async Task<List<string>> GetChatFriends(string chatName)
        {
            List<string> totalList = new();
            try
            {
                var currentChatsList = await _context.Chats.Where(u => u.SenderChatName == chatName).AsNoTracking().ToListAsync();
                foreach (Message user in currentChatsList)
                {
                    if (user.ReceiverChatName != null && !totalList.Contains(user.ReceiverChatName))
                    {
                        totalList.Add(user.ReceiverChatName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Grabbing Chat Friends List: {ex}", ex);
            }

            return totalList;
        }


        public async Task<List<Message>> GrabFriendChats(string chatName)
        {
            List<Message> chats = new List<Message>();
            try
            {
                chats = await _context.Chats.Where(u => u.ReceiverChatName == chatName).AsNoTracking().ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError("Error Getting Chats From Friends: {ex}", ex);
            }
            return chats;

        }

        public async Task<List<Message>> AddQueuedMessages(string username)
        {
            List<Message> messages = new List<Message>();
            List<MessageQueue> queues = new List<MessageQueue>();

            try
            {
                queues = await _context.Queues.Where(u => u.ReceiverChatName == username).ToListAsync();
                foreach (MessageQueue queue in queues)
                {
                    queue.MessageStatus = Status.Sent.ToString();
                    var newMsg = CreateMessageObject(queue);
                    messages.Add(newMsg);
                    await _context.Chats.AddAsync(newMsg);
                    _context.Queues.Remove(queue);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Adding Queued Message: {ex}", ex);
            }

            return messages;
        }

        public void InsertMessage(Message chat)
        {
            _context.Chats.Add(chat);
            _context.SaveChanges();
        }

        public void InsertQueuedMessage(MessageQueue queuedMessage)
        {
            try
            {
                _context.Queues.Add(queuedMessage);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public Message CreateMessageObject(MessageQueue queue)
        {
            var newMessage = new MessageBuilder()
                                .SetMessageStatus(queue.MessageStatus)
                                .SetMessage(queue.UserMessage)
                                .SetMessageDate(DateTime.Now)
                                .SetIsReceived(queue.IsReceived)
                                .SetSenderEmail(queue.MessageSenderEmail)
                                .SetSenderChatName(queue.SenderChatName)
                                .SetReceiverEmail(queue.MessageReceiverEmail)
                                .SetReceiverChatName(queue.ReceiverChatName)
                                .Build();

            return newMessage;
        }
        public void CreateMessageObject(string message,
                                        string senderEmail,
                                        string receiverEmail,
                                        Enum status,
                                        string senderChatName,
                                        string receiverChatName)
        {
            string messageFamilyUID = Guid.NewGuid().ToString();

            var newMessage = new MessageBuilder()
                                .SetMessageStatus(status.ToString())
                                .SetMessage(message)
                                .SetMessageDate(DateTime.Now)
                                .SetIsReceived(true)
                                .SetSenderEmail(senderEmail)
                                .SetSenderChatName(senderChatName)
                                .SetReceiverEmail(receiverEmail)
                                .SetReceiverChatName(receiverChatName)
                                .Build();

            InsertMessage(newMessage);

        }

        public void CreateQueuedMessageObject(string message,
                                              string senderEmail,
                                              string receiverEmail,
                                              Enum status,
                                              string senderChatName,
                                              string receiverChatName)
        {
            string messageFamilyUID = Guid.NewGuid().ToString();


            var queuedMessage = new MessageQueueBuilder()
                    .SetMessageStatus(status.ToString())
                    .SetMessage(message)
                    .SetMessageDate(DateTime.Now)
                    .SetIsReceived(true)
                    .SetSenderEmail(senderEmail)
                    .SetSenderChatName(senderChatName)
                    .SetReceiverEmail(receiverEmail)
                    .SetReceiverChatName(receiverChatName)
                    .Build();

            InsertQueuedMessage(queuedMessage);
        }

        public MessageQueue CreateQueuedMessageObject(Message msg)
        {
            var queuedMessage = new MessageQueueBuilder()
                    .SetMessageStatus(msg.MessageStatus)
                    .SetMessage(msg.UserMessage)
                    .SetMessageDate(DateTime.Now)
                    .SetIsReceived(msg.IsReceived)
                    .SetSenderEmail(msg.MessageSenderEmail)
                    .SetSenderChatName(msg.SenderChatName)
                    .SetReceiverEmail(msg.MessageReceiverEmail)
                    .SetReceiverChatName(msg.ReceiverChatName)
                    .Build();

            return queuedMessage;

        }

    }
}


