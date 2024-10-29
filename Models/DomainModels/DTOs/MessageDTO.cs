using WindTalkerMessenger.Models.DataLayer;

namespace WindTalkerMessenger.Models.DomainModels.DTOs
{
    public class MessageDTO
    {
        public int MessageId { get; set; }
        public string? UserMessage { get; set; }
        public DateTime MessageDate { get; set; } = DateTime.Now;
        public string? MessageSenderChatName { get; set; }
        public string? MessageReceiverChatName { get; set; }
    }
}
