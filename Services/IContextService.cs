using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public interface IContextService
    {
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
        Task DisassociateIdentityUserMessagesAsync(string identityUserEmail);
        void InsertMessage(Message message);
        Task DisassociateGuestUserMessagesAsync(string guestChatName);


		void AddNewGuest(string userName, string connectionId);

        Task DisassociateIdentityUserQueuedMessagesAsync(string identityUserEmail);

    }
}

