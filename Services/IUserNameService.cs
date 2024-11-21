namespace WindTalkerMessenger.Services
{
    public interface IUserNameService
    {
        bool IsUserNameAvailable(string guestName);
        string GetSenderChatName(string senderConnectionId);
        string GetReceiverIdentityEmail(string receiverConnectionId);
        bool IsUserAuthenticated();
        List<string> GetAllUserNames();

        Task KillSwitchAsync(string userId);

        Task<bool> IsUserRegistered(string username);


    }
}
