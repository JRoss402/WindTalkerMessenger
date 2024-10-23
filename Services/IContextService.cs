using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public interface IContextService
    {
        //Task CreateNewMessage(Message Chat, string userName);

        //Can the object creation methods all be void?
        void CreateMessageObject(string message, 
                                 string senderEmail, 
                                 string receiverEmail, 
                                 string messageFamilyUID,
                                 Enum status, 
                                 string senderChatName, 
                                 string receiverChatName);
        Message CreateMessageObject(MessageQueue queue);
        MessageQueue CreateQueuedMessageObject(Message message);
        void CreateQueuedMessageObject(string message, 
                                       string senderEmail, 
                                       string receiverEmail, 
                                       string messageFamilyUID,
                                       Enum status, 
                                       string senderChatName, 
                                       string receiverChatName);

        Task<List<Message>> GetReceivedMessages();
        Task<List<Message>> SendQueuedMessages();

        void IsRowRemovable(Message message);
        void DisassociateUserMessages(string identityUserEmail);
        void InsertMessage(Message message);

        void AddNewGuest(string userName, string connectionId);





    }
}

