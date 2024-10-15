using WindTalkerMessenger.Models.DataLayer;

namespace WindTalkerMessenger.Models.DomainModels
{
    public class ChatMessage
    {
        public int ChatMessageId { get; set; }
        public string? MessageUID { get; set; }

        public string? MessageStatus { get; set; } //Sent,Received,Queued,Read
        public string? UserMessage { get; set; }
        public DateTime MessageDate { get; set; } = DateTime.Now;
        public bool isLoaded { get; set; } = false;


        public string? MsgSenderEmail { get; set; }

        public ApplicationUser? IdentitySender { get; set; }
        public string? MsgReceiverEmail { get; set; }
        public ApplicationUser? IdentityReceiver { get; set; }

    }
}
