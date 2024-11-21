using WindTalkerMessenger.Models.DataLayer;

namespace WindTalkerMessenger.Models.DomainModels
{
    public class MessageQueue
    {
        public int MessageQueueId { get; set; }
        public string? MessageStatus { get; set; }
        public string? UserMessage { get; set; }
        public DateTime MessageDate { get; set; } = DateTime.Now;
        public bool IsReceived { get; set; } = false;
        public string? MessageSenderEmail { get; set; }
        public string? MessageReceiverEmail { get; set; }
        public string? SenderChatName { get; set; }
        public string? ReceiverChatName { get; set; }

    }
}
