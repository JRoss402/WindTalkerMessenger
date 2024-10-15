using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public class ContextService : IContextService
    {
        private readonly ApplicationDbContext _context;
        private readonly OnlineUsersLists _onlineUsersLists;
        private readonly ICacheService _cacheService;

        public ContextService(ApplicationDbContext context,OnlineUsersLists onlineUsersLists,
                              ICacheService cacheService)
        {
            _context = context;
            _onlineUsersLists = onlineUsersLists;
            _cacheService = cacheService;
        }

        enum Status
        {
            //Message passed through SendMessage method
            Sent,
            //Message was received in real-time
            Received,
            //The message was placed in a message que
            Queued,
        }

        public bool CheckGuestName(string guestName)
        {
            bool isTaken;

            if (!_onlineUsersLists.anonUsers.Contains(guestName))
            {
                isTaken = false;
            }
            else
            {
                isTaken = true;
            }

            return isTaken;
        }

        public async Task<List<ChatMessage>> GrabAllChats()
        {
            var chats = await _context.Chats.AsNoTracking().ToListAsync();

            return chats;
        }

        public async Task GuestHashRemovalAsync(string connectionId)
        {
            
        }
        public async Task AddDbMessage(ChatMessage msg, string guestName)
        {
            if(!_onlineUsersLists.onlineUsers.Contains(msg.MsgReceiverEmail) ||
               !_onlineUsersLists.onlineUsers.Contains(guestName))
            {
                MessageQueue queue = new MessageQueue();

                queue = CreateQueueObject(msg);
                await _context.Queues.AddAsync(queue);
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.Chats.AddAsync(msg);
                await _context.SaveChangesAsync();
            }
        }

       /* public async Task AddMetaData(MsgMetaData meta)
        {
            MsgMetaData metaData = new MsgMetaData();

            metaData.Message = meta.Message;
            metaData.MsgSender = meta.MsgSender;
            metaData.MsgReceiver = meta.MsgReceiver;
            metaData.ChatStatusID = meta.ChatStatusID;
            metaData.TimeStamp = meta.TimeStamp;
            metaData.MsgStatus = meta.MsgStatus;
            try
            {
                await _context.MsgMetaData.AddAsync(metaData);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }*/



        public async Task<List<ChatMessage>> GrabNewChats()
        {
            var readList = _cacheService.CacheChats().Result.Where(e => e.isLoaded == false).ToList();

            return readList;
        }

        
        public async Task<List<ChatMessage>> CheckMsgQueue()
        {
            var queues = _context.Queues;
            if (queues != null)
            {
                foreach (MessageQueue queue in queues)
                {
                     CreateChatObject(queue);

                    _context.Queues.Remove(queue);
                    _context.SaveChanges();
                }
            }
            return null;
        }

        public void AddChatObject(ChatMessage chat)
        {
            _context.Chats.Add(chat);
            _context.SaveChanges();
        }

        public void CreateChatObject(MessageQueue queue)
        {
            ChatMessage chat = new ChatMessage();

            chat.MessageDate = DateTime.Now;
            chat.UserMessage = queue.UserMessage;
            chat.MessageStatus = queue.MessageStatus;
            chat.MsgSenderEmail = queue.MsgSenderEmail;
            chat.MsgReceiverEmail = queue.MsgReceiverEmail;
            chat.MessageUID = queue.MessageUID;
            chat.isLoaded = queue.isLoaded;

            AddChatObject(chat);

        }
        public ChatMessage CreateChatObject(string message, string receiverUser, string senderUser, string ChatUID, Enum status )
        {
            ChatMessage chat = new ChatMessage();
            chat.MessageDate = DateTime.Now;
            chat.UserMessage = message;
            chat.MessageStatus = status.ToString();
            chat.MsgSenderEmail = senderUser;
            chat.MsgReceiverEmail = receiverUser;
            chat.MessageUID = ChatUID;
            chat.isLoaded = true;
            AddChatObject(chat);

            return chat;
        }

        /*public MsgMetaData CreateMetaObject(string message, string senderEmail, string receiverEmail, string statusId)
        {
            MsgMetaData meta = new MsgMetaData();

            meta.MsgStatus = Status.Sent.ToString();
            meta.Message = message;
            meta.MsgSender = senderEmail;
            meta.MsgReceiver = receiverEmail;
            meta.ChatStatusID = statusId;
            meta.TimeStamp = DateTime.Now;

            return meta;
        }*/

        public MessageQueue CreateQueueObject(string message, string senderEmail, string receiverEmail, string msgUID, Enum status)
        {
            MessageQueue queue = new MessageQueue();

            queue.MessageDate = DateTime.Now;
            queue.UserMessage = message;
            queue.MessageStatus = status.ToString();
            queue.MsgSenderEmail = senderEmail;
            queue.MsgReceiverEmail = receiverEmail;
            queue.MessageUID = msgUID;
            queue.isLoaded = queue.isLoaded = false;

            return queue;
        }

        public MessageQueue CreateQueueObject(ChatMessage msg)
        {
            MessageQueue queue = new MessageQueue();

            queue.MessageDate = DateTime.Now;
            queue.UserMessage = msg.UserMessage;
            queue.MessageStatus = msg.MessageStatus;
            queue.MsgSenderEmail = msg.MsgSenderEmail;
            queue.MsgReceiverEmail = msg.MsgReceiverEmail;
            queue.MessageUID = msg.MessageUID;
            queue.isLoaded = msg.isLoaded;

            return queue;
        }

    }
}


