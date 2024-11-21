namespace WindTalkerMessenger.Models.DomainModels
{
    public class MessageBuilder
    {

        private Message _message = new Message();

        public MessageBuilder WithMessageStatus(string messageStatus)
        {
            _message.MessageStatus = messageStatus;
            return this;
        }
        
        public MessageBuilder WithMessage(string message)
        {
            _message.UserMessage = message;
            return this;
        }

        public MessageBuilder WithMessageDate(DateTime messageDate)
        {
            _message.MessageDate = messageDate;
            return this;
        }

        public MessageBuilder WithIsReceived(bool isReceived)
        {
            _message.IsReceived = isReceived;
            return this;
        }

        public MessageBuilder WithSenderEmail(string  senderEmail)
        {
            _message.MessageSenderEmail = senderEmail;
            return this;
        }

        public MessageBuilder WithReceiverEmail(string receiverEmail)
        {
            _message.MessageReceiverEmail = receiverEmail;
            return this;
        }

        public MessageBuilder WithSenderChatName(string senderChatName)
        {
            _message.SenderChatName = senderChatName;
            return this;
        }

        public MessageBuilder WithReceiverChatName(string receiverChatName)
        {
            _message.ReceiverChatName = receiverChatName;
            return this;
        }

        public Message Build()
        {

            return _message;
        }

    }
}
