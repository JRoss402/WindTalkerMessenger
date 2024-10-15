using WindTalkerMessenger.Models.DataLayer;

namespace WindTalkerMessenger.Models.DomainModels
{
    public class MessageQueue
    {
        public int MessageQueueId { get; set; }
        public string? MessageUID { get; set; }
        public string? MessageStatus { get; set; }
        public string? UserMessage { get; set; }
        public DateTime MessageDate { get; set; } = DateTime.Now;
        public bool isLoaded { get; set; } = false;
        public string? MsgSenderEmail { get; set; }

        public ApplicationUser? IdentitySender { get; set; }
        public string? MsgReceiverEmail { get; set; }
        public ApplicationUser? IdentityReceiver { get; set; }

    }
}
