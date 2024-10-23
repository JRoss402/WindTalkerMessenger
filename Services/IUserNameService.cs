namespace WindTalkerMessenger.Services
{
    public interface IUserNameService
    {
        bool IsUserNameAvailable(string guestName);
        string GetSenderChatName(string senderConnectionId);
        string GetReceiverEmail(string receiverConnectionId);
    }
}
