using Microsoft.EntityFrameworkCore;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public class ContextService : IContextService
    {
        private readonly ApplicationDbContext _context;
        private readonly OnlineUsersLists _onlineUsersLists;
        private const string DELETED = "User Account Deleted";

        public ContextService(ApplicationDbContext context, 
                              OnlineUsersLists onlineUsersLists)
        {
            _context = context;
            _onlineUsersLists = onlineUsersLists;
        }

        public void DissociateUserMessages(string identityUserEmail) 
        {
            var userMessages =  _context.Chats.Where(e => e.MessageSenderEmail == identityUserEmail ||
                                                          e.MessageReceiverEmail == identityUserEmail).ToList();

            foreach(var userMessage in userMessages)
            {
                if(userMessage.MessageSenderEmail == identityUserEmail)
                {
                    userMessage.MessageSenderEmail = DELETED;
                }
                else if(userMessage.MessageReceiverEmail == identityUserEmail)
                {
                    userMessage.MessageReceiverEmail = DELETED;
                }

                IsRowRemovable(userMessage);

                _context.Chats.Update(userMessage);
            }
            _context.SaveChanges();
              
        }

        public void IsRowRemovable(Message message)
        {
            if(message.MessageSenderEmail == DELETED &&
               message.MessageReceiverEmail == DELETED)
            {
                _context.Chats.Remove(message);
            }
        }

        enum Statuses
        {
            Sent, Received, Queued,
        }

        public async Task<List<Message>> GetReceivedMessages()
        {
            var chats = await _context.Chats.AsNoTracking().ToListAsync();

            return chats;
        }

        public async Task CreateNewMessage(Message msg, string guestName)
        {
            if(!_onlineUsersLists.onlineUsers.Contains(msg.MessageReceiverEmail) ||
               !_onlineUsersLists.onlineUsers.Contains(guestName))
            {
                MessageQueue queue = new MessageQueue();

                queue = CreateQueuedMessageObject(msg);
                await _context.Queues.AddAsync(queue);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.Chats.AddAsync(msg);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Message>> SendQueuedMessages()
        {
            //The new chats objects are being created, but why is this returning a list?

            var queues = _context.Queues;
            if (queues != null)
            {
                foreach (MessageQueue queue in queues)
                {
                    CreateMessageObject(queue);

                    _context.Queues.Remove(queue);
                    _context.SaveChanges();
                }
            }
            return null;
        }

        public void InsertMessage(Message chat)
        {
            _context.Chats.Add(chat);
            _context.SaveChanges();
        }

        public Message CreateMessageObject(MessageQueue queue)
        {
            Message chat = new Message();

            chat.MessageDate = DateTime.Now;
            chat.UserMessage = queue.UserMessage;
            chat.MessageStatus = queue.MessageStatus;
            chat.MessageSenderEmail = queue.MessageSenderEmail;
            chat.MessageReceiverEmail = queue.MessageReceiverEmail;
            chat.MessageUID = queue.MessageUID;
            chat.IsReceived = queue.IsReceived;

            return chat;
        }
        public void CreateMessageObject(string message, string receiverUser, string senderUser, string ChatUID, Enum status )
        {
            Message chat = new Message();
            chat.MessageDate = DateTime.Now;
            chat.UserMessage = message;
            chat.MessageStatus = status.ToString();
            chat.MessageSenderEmail = senderUser;
            chat.MessageReceiverEmail = receiverUser;
            chat.MessageUID = ChatUID;
            chat.IsReceived = true;
            InsertMessage(chat);
        }


        public MessageQueue CreateQueuedMessageObject(string message, string senderEmail, string receiverEmail, string msgUID, Enum status)
        {
            MessageQueue queue = new MessageQueue();

            queue.MessageDate = DateTime.Now;
            queue.UserMessage = message;
            queue.MessageStatus = status.ToString();
            queue.MessageSenderEmail = senderEmail;
            queue.MessageReceiverEmail = receiverEmail;
            queue.MessageUID = msgUID;
            queue.IsReceived = queue.IsReceived = false;

            return queue;
        }

        public MessageQueue CreateQueuedMessageObject(Message msg)
        {
            MessageQueue queue = new MessageQueue();

            queue.MessageDate = DateTime.Now;
            queue.UserMessage = msg.UserMessage;
            queue.MessageStatus = msg.MessageStatus;
            queue.MessageSenderEmail = msg.MessageSenderEmail;
            queue.MessageReceiverEmail = msg.MessageReceiverEmail;
            queue.MessageUID = msg.MessageUID;
            queue.IsReceived = msg.IsReceived;

            return queue;
        }

	}
}


