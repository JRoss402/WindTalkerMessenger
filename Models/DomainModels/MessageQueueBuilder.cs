namespace WindTalkerMessenger.Models.DomainModels
{
    public class MessageQueueBuilder
    {
        private MessageQueue _messageQueue = new MessageQueue();
        public MessageQueueBuilder SetMessageStatus(string messageStatus)
        {
            _messageQueue.MessageStatus = messageStatus;
            return this;
        }

        public MessageQueueBuilder SetMessage(string message)
        {
            _messageQueue.UserMessage = message;
            return this;
        }

        public MessageQueueBuilder SetMessageDate(DateTime messageDate)
        {
            _messageQueue.MessageDate = messageDate;
            return this;
        }

        public MessageQueueBuilder SetIsReceived(bool isReceived)
        {
            _messageQueue.IsReceived = isReceived;
            return this;
        }

        public MessageQueueBuilder SetSenderEmail(string senderEmail)
        {
            _messageQueue.MessageSenderEmail = senderEmail;
            return this;
        }

        public MessageQueueBuilder SetReceiverEmail(string receiverEmail)
        {
            _messageQueue.MessageReceiverEmail = receiverEmail;
            return this;
        }

        public MessageQueueBuilder SetSenderChatName(string senderChatName)
        {
            _messageQueue.SenderChatName = senderChatName;
            return this;
        }

        public MessageQueueBuilder SetReceiverChatName(string receiverChatName)
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
