using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public interface IContextService
    {

        Task<HashSet<string>> GetChatNames();

        void CreateMessageObject(string message, 
                                 string senderEmail, 
                                 string receiverEmail, 
                                 Enum status, 
                                 string senderChatName, 
                                 string receiverChatName);
        Message CreateMessageObject(MessageQueue queue);
        MessageQueue CreateQueuedMessageObject(Message message);
        void CreateQueuedMessageObject(string message, 
                                       string senderEmail, 
                                       string receiverEmail, 
                                       Enum status, 
                                       string senderChatName, 
                                       string receiverChatName);

        Task<List<Message>> AddQueuedMessages(string username);

		Task<List<string>> GetChatFriends(string chatName);

        void IsQueueRowRemovable(MessageQueue queue);

        void IsMessageRowRemovable(Message message);
        Task UpdateIdentityMessagesAsync(string identityUserEmail);
        void InsertMessage(Message message);
        Task UpdateGuestMessagesAsync(string guestChatName);

		void AddNewGuest(string userName, string connectionId);

        Task DeleteIdentityQueuedMessagesAsync(string identityUserEmail);

    }
}

