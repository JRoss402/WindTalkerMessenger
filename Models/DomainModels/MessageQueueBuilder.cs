namespace WindTalkerMessenger.Models.DomainModels
{
    public class MessageQueueBuilder
    {


        private MessageQueue _messageQueue = new MessageQueue();

        public MessageQueueBuilder WithMessageStatus(string messageStatus)
        {
            _messageQueue.MessageStatus = messageStatus;
            return this;
        }

        public MessageQueueBuilder WithMessage(string message)
        {
            _messageQueue.UserMessage = message;
            return this;
        }

        public MessageQueueBuilder WithMessageFamilyUID(string messageFamilyUID)
        {
            _messageQueue.MessageFamilyUID = messageFamilyUID;
            return this;
        }

        public MessageQueueBuilder WithMessageDate(DateTime messageDate)
        {
            _messageQueue.MessageDate = messageDate;
            return this;
        }

        public MessageQueueBuilder WithIsReceived(bool isReceived)
        {
            _messageQueue.IsReceived = isReceived;
            return this;
        }

        public MessageQueueBuilder WithSenderEmail(string senderEmail)
        {
            _messageQueue.MessageSenderEmail = senderEmail;
            return this;
        }

        public MessageQueueBuilder WithReceiverEmail(string receiverEmail)
        {
            _messageQueue.MessageReceiverEmail = receiverEmail;
            return this;
        }

        public MessageQueueBuilder WithSenderChatName(string senderChatName)
        {
            _messageQueue.SenderChatName = senderChatName;
            return this;
        }

        public MessageQueueBuilder WithReceiverChatName(string receiverChatName)
        {
            _messageQueue.ReceiverChatName = receiverChatName;
            return this;
        }

        public MessageQueue Build()
        {

            return _messageQueue;
        }
        /*public int MessageId { get; set; }
        public string? MessageStatus { get; set; }
        public string? UserMessage { get; set; }
        public string? MessageFamilyUID { get; set; }
        public DateTime MessageDate { get; set; } = DateTime.Now;
        public bool IsReceived { get; set; } = false;
        public string? MessageSenderEmail { get; set; }
        public string? MessageReceiverEmail { get; set; }
        public string? SenderChatName { get; set; }
        public string? ReceiverChatName { get; set; }
    }*/

    }
}
