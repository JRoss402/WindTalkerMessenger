
namespace WindTalkerMessenger.Models.DomainModels
{
    public class Message
    {
        public int MessageId { get; set; }
        public string? MessageStatus { get; set; }
        public string? UserMessage { get; set; }
        public DateTime MessageDate { get; set; } = DateTime.Now;
        public string? MessageFamilyUID { get; set; }

        public bool IsReceived { get; set; } = false;
        public string? MessageSenderEmail { get; set; }
        public string? MessageReceiverEmail { get; set; }
        public string? SenderChatName { get; set; }
        public string? ReceiverChatName { get; set; }
    }
}
