using WindTalkerMessenger.Models.DataLayer;
using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public interface IContextService
    {
        Task AddDbMessage(ChatMessage Chat, string guestName);
        Task<List<ChatMessage>> GrabAllChats();
        Task<List<ChatMessage>> CheckMsgQueue();
        Task<List<ChatMessage>> GrabNewChats();
        public string GradIdentityUserName();

        Task GuestHashRemovalAsync(string connectionId);
        //Task<List<ApplicationUser>> GrabAllUserNames();
        Task<bool> UserNameCheck(string userName);


        public void CreateChatObject(string message, string receiverUser, string senderUser, string ChatUID, Enum status);

		//ChatMessage CreateChatObject(string message, string receiverUser, string senderUser, string ChatUID, Enum status);
        //void CreateChatObject(MessageQueue queue);
        MessageQueue CreateQueueObject(string message, string senderEmail, string receiverEmail, string ChatUID, Enum status);


    }
}

