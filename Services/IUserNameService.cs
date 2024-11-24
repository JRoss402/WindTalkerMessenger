namespace WindTalkerMessenger.Services
{
    public interface IUserNameService
    {
        bool IsUserNameAvailable(string guestName);
        string GetSenderChatName(string senderConnectionId);
        string GetReceiverIdentityEmail(string receiverConnectionId);
        bool IsUserAuthenticated();
        Task<List<string>> GetAllUserNames();
		Task KillSwitchAsync(string userId);
        Task<bool> IsUserRegistered(string username);

        //Task AddNewGuestAsync(string chatName, string connectionId);
        //Task AddNewRegisteredUserAsync(string chatName, string connectionId);

    }
}
