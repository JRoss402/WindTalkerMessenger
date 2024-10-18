using WindTalkerMessenger.Models.DomainModels;

namespace WindTalkerMessenger.Services
{
    public interface IContextService
    {
        Task CreateNewMessage(Message Chat, string userName);

        //Can the object creation methods all be void?
        void CreateMessageObject(string message, string receiverUser, string senderUser, string ChatUID, Enum statuses);
        Message CreateMessageObject(MessageQueue queue);
        MessageQueue CreateQueuedMessageObject(Message message);
        MessageQueue CreateQueuedMessageObject(string message, string senderEmail, string receiverEmail, string ChatUID, Enum statuses);

        Task<List<Message>> GetReceivedMessages();
        Task<List<Message>> SendQueuedMessages();

        void IsRowRemovable(Message message);
        void DissociateUserMessages(string identityUserEmail);
        void InsertMessage(Message message);






    }
}

