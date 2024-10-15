using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public interface IContextService
    {
        Task AddDbMessage(ChatMessage Chat, string guestName);
        bool CheckGuestName(string guestName);
        Task<List<ChatMessage>> GrabAllChats();
        Task<List<ChatMessage>> CheckMsgQueue();
        Task<List<ChatMessage>> GrabNewChats();
        Task GuestHashRemovalAsync(string connectionId);


		ChatMessage CreateChatObject(string message, string receiverUser, string senderUser, string ChatUID, Enum status);
        void CreateChatObject(MessageQueue queue);
        MessageQueue CreateQueueObject(string message, string senderEmail, string receiverEmail, string ChatUID, Enum status);

        //MsgMetaData CreateMetaObject(string message, string senderEmail,string receiverEmail,string statusId);
        //Task AddMetaData(MsgMetaData meta);

    }
}

