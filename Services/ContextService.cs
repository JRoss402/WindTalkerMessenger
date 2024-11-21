using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
	public class ContextService : IContextService
	{
		enum Status
		{
			Sent, Received, Queued
		}


		private readonly ApplicationDbContext _context;
		private readonly OnlineUsersLists _onlineUsersLists;
		private readonly IHttpContextAccessor _http;
		private readonly UserManager<ApplicationUser> _userManager;
		private const string USER_DELETED = "User Account Deleted";
		private const string MSG_DELETED = "User Deleted Messages";
        private readonly ILogger<ContextService> _logger;
        private enum Statuses { Sent, Received, Queued }

		public ContextService(ApplicationDbContext context,
							  OnlineUsersLists onlineUsersLists,
							  UserManager<ApplicationUser> userManager,
							  IHttpContextAccessor http,
							  ILogger<ContextService> logger)
		{
			_context = context;
			_onlineUsersLists = onlineUsersLists;
			_userManager = userManager;
			_http = http;
			_logger = logger;
		}

		public void AddNewGuest(string userName,
								string connectionId)
		{
			Guest guest = new Guest();
			string guestuid = Guid.NewGuid().ToString();
			guest.GuestName = userName;
			guest.GuestConnectionId = connectionId;
			guest.GuestUID = guestuid;

			_context.Guests.Add(guest);
			_context.SaveChanges();
		}

		public void DisassociateGuestUserMessages(string guestChatName)
		{
			var chats = _context.Chats.Where(u => u.SenderChatName == guestChatName ||
									  u.ReceiverChatName == guestChatName).ToList();
						  
			foreach(Message chat in chats)
			{
				if(chat.SenderChatName == guestChatName)
				{
					chat.SenderChatName = "Guest Disconnected";
					chat.UserMessage = "Guest Disconnected";
					//IsRowRemovable(chat);
					
				}else if(chat.ReceiverChatName == guestChatName)
				{
					chat.ReceiverChatName = "Guest Disconnected";
					//IsRowRemovable(chat);

				}
				_context.SaveChangesAsync();
			}
		}

		public void DisassociateIdentityUserMessages(string identityUserEmail)
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
			_context.SaveChanges();

			DisassociateIdentityUserQueuedMessages(identityUserEmail);

        }

		public void DisassociateIdentityUserQueuedMessages(string identityUserEmail)
		{
            var userQueuedMessages = _context.Queues.Where(e => e.MessageSenderEmail == identityUserEmail ||
                                              e.MessageReceiverEmail == identityUserEmail).ToList();

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
            _context.SaveChanges();


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
			List<string> totalList = new List<string>();
			try
			{
				var currentChatsList = await _context.Chats.Where(u => u.SenderChatName == chatName).ToListAsync();
				foreach (Message user in currentChatsList)
				{
					if (!totalList.Contains(user.ReceiverChatName))
					{
						totalList.Add(user.ReceiverChatName);
					}
				}
			}catch(Exception ex)
			{
				_logger.LogError(ex.ToString());
			}

			return totalList;
		}


		public async Task<List<Message>> GrabFriendChats(string chatName)
		{
			List<Message> chats = new List<Message>();
			try
			{
				chats = await _context.Chats.Where(u => u.ReceiverChatName == chatName).ToListAsync();
            
			}catch(Exception ex)
			{
				_logger.LogError(ex.ToString());
			}
            return chats;

        }

        /*public bool IsUserGuest(string chatName)
		{
			bool isGuest = _onlineUsersLists.anonUsers.ContainsKey(chatName);
			if (isGuest)
			{
				return true;
			}
			else
			{
				return false;
			}

		}*/

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
            catch(Exception ex)
			{
				_logger.LogError(ex.ToString());
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
								.WithMessageStatus(queue.MessageStatus)
								.WithMessage(queue.UserMessage)
								//.WithMessageFamilyUID(queue.MessageFamilyUID)
								.WithMessageDate(DateTime.Now)
								.WithIsReceived(queue.IsReceived)
								.WithSenderEmail(queue.MessageSenderEmail)
								.WithSenderChatName(queue.SenderChatName)
								.WithReceiverEmail(queue.MessageReceiverEmail)
								.WithReceiverChatName(queue.ReceiverChatName)
								.Build();

			return newMessage;
		}
		public void CreateMessageObject(string message,
										string senderEmail,
										string receiverEmail,
										//string messageFamilyUID,
										Enum status,
										string senderChatName,
										string receiverChatName)
		{
			string messageFamilyUID = Guid.NewGuid().ToString();

			var newMessage = new MessageBuilder()
								.WithMessageStatus(status.ToString())
								.WithMessage(message)
								//.WithMessageFamilyUID(messageFamilyUID)
								.WithMessageDate(DateTime.Now)
								.WithIsReceived(true)
								.WithSenderEmail(senderEmail)
								.WithSenderChatName(senderChatName)
								.WithReceiverEmail(receiverEmail)
								.WithReceiverChatName(receiverChatName)
								.Build();

			InsertMessage(newMessage);

		}

		public void CreateQueuedMessageObject(string message,
											  string senderEmail,
											  string receiverEmail,
											  //string messageFamilyUID,
											  Enum status,
											  string senderChatName,
											  string receiverChatName)
		{
			string messageFamilyUID = Guid.NewGuid().ToString();


			var queuedMessage = new MessageQueueBuilder()
					.WithMessageStatus(status.ToString())
					.WithMessage(message)
					//.WithMessageFamilyUID(messageFamilyUID)
					.WithMessageDate(DateTime.Now)
					.WithIsReceived(true)
					.WithSenderEmail(senderEmail)
					.WithSenderChatName(senderChatName)
					.WithReceiverEmail(receiverEmail)
					.WithReceiverChatName(receiverChatName)
					.Build();

			InsertQueuedMessage(queuedMessage);
		}

		public MessageQueue CreateQueuedMessageObject(Message msg)
		{
			var queuedMessage = new MessageQueueBuilder()
					.WithMessageStatus(msg.MessageStatus)
					.WithMessage(msg.UserMessage)
					//.WithMessageFamilyUID(msg.MessageFamilyUID)
					.WithMessageDate(DateTime.Now)
					.WithIsReceived(msg.IsReceived)
					.WithSenderEmail(msg.MessageSenderEmail)
					.WithSenderChatName(msg.SenderChatName)
					.WithReceiverEmail(msg.MessageReceiverEmail)
					.WithReceiverChatName(msg.ReceiverChatName)
					.Build();

			return queuedMessage;
		}

	}
}


