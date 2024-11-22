namespace WindTalkerMessenger.Models.DomainModels
{
    public class MessageBuilder
    {

        private Message _message = new Message();

        public MessageBuilder SetMessageStatus(string messageStatus)
        {
            _message.MessageStatus = messageStatus;
            return this;
        }
        
        public MessageBuilder SetMessage(string message)
        {
            _message.UserMessage = message;
            return this;
        }

        public MessageBuilder SetMessageDate(DateTime messageDate)
        {
            _message.MessageDate = messageDate;
            return this;
        }

        public MessageBuilder SetIsReceived(bool isReceived)
        {
            _message.IsReceived = isReceived;
            return this;
        }

        public MessageBuilder SetSenderEmail(string  senderEmail)
        {
            _message.MessageSenderEmail = senderEmail;
            return this;
        }

        public MessageBuilder SetReceiverEmail(string receiverEmail)
        {
            _message.MessageReceiverEmail = receiverEmail;
            return this;
        }

        public MessageBuilder SetSenderChatName(string senderChatName)
        {
            _message.SenderChatName = senderChatName;
            return this;
        }

        public MessageBuilder SetReceiverChatName(string receiverChatName)
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
