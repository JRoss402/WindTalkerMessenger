namespace WindTalkerMessenger.Models.DomainModels
{
    public class Receiver : IUserInformation
    {
        public string? ConnectionId { get; set; }
        public string? ChatName { get; set; }
        public string? EMail { get; set; }
    }
}
