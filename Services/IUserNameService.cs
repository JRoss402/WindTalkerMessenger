namespace WindTalkerMessenger.Services
{
    public interface IUserNameService
    {
        bool IsUserNameAvailable(string guestName);
        string GetIdentityChatName(string senderConnectionId);

    }
}
