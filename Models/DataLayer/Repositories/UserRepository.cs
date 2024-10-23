using Microsoft.EntityFrameworkCore;
using WindTalkerMessenger.Models.DomainModels;
using WindTalkerMessenger.Services;

namespace WindTalkerMessenger.Models.DataLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private const string DELETED = "User Account Deleted";
        private readonly IContextService _contextService;
        private enum Statuses { Sent, Received, Queued }

        public UserRepository(ApplicationDbContext context,
                              IContextService contextService)
        {
            _context = context;
            _contextService = contextService;
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

        public void RowBackFill()
        {

        }

        public void IsRowRemovable(Message message)
        {
            if (message.MessageSenderEmail == DELETED &&
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

        public async Task<List<Message>> SendQueuedMessages(string receiverChatName)
        {
            //The new chats objects are being created, but why is this returning a list?

            var queues = _context.Queues.Where(u => u.ReceiverChatName == receiverChatName).ToList();
            if (queues != null)
            {
                foreach (MessageQueue queue in queues)
                {
                    _contextService.CreateMessageObject(queue);
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

    }
}
