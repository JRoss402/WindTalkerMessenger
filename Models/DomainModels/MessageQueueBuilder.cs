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

    }
}
