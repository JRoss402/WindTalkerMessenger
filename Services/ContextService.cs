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
        private enum Statuses { Sent, Received, Queued }

        public ContextService(ApplicationDbContext context, 
                              OnlineUsersLists onlineUsersLists)
        {
            _context = context;
            _onlineUsersLists = onlineUsersLists;
        }

        public void AddNewGuest(string userName, 
                                string connectionId)
        {
            Guest guest = new Guest();
            guest.GuestName = userName;
            guest.GuestConnectionId = connectionId;

            _context.Add(guest);
            _context.SaveChanges();
        }

        public void DisassociateUserMessages(string identityUserEmail) 
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
        public void RowBackFill()
        {
            
        }

        public void IsRowRemovable(Message message)
        {
            if(message.MessageSenderEmail == DELETED &&
               message.MessageReceiverEmail == DELETED)
            {
                _context.Chats.Remove(message);
            }
        }


        public async Task<List<Message>> GetReceivedMessages()
        {
            var chats = await _context.Chats.AsNoTracking().ToListAsync();

            return chats;
        }

        public async Task<List<Message>> SendQueuedMessages()
        {
            //Fix the header or return

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

        public void InsertQueuedMessage(MessageQueue queuedMessage)
        {
            _context.Queues.Add(queuedMessage);
            _context.SaveChanges();
        }

        public Message CreateMessageObject(MessageQueue queue)
        {
            var newMessage = new MessageBuilder()
                                .WithMessageStatus(queue.MessageStatus)
                                .WithMessage(queue.UserMessage)
                                .WithMessageFamilyUID(queue.MessageFamilyUID)
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
                                        string messageFamilyUID,
                                        Enum status, 
                                        string senderChatName, 
                                        string receiverChatName)
        {
            var newMessage = new MessageBuilder()
                                .WithMessageStatus(status.ToString())
                                .WithMessage(message)
                                .WithMessageFamilyUID(messageFamilyUID)
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
                                              string messageFamilyUID, 
                                              Enum status, 
                                              string senderChatName, 
                                              string receiverChatName)
        {

            var queuedMessage = new MessageQueueBuilder()
                    .WithMessageStatus(status.ToString())
                    .WithMessage(message)
                    .WithMessageFamilyUID(messageFamilyUID)
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
                    .WithMessageFamilyUID(msg.MessageFamilyUID)
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


