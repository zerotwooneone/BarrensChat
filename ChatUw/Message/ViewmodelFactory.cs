namespace ChatUw.Message
{
    public class ViewmodelFactory : IMessageViewmodelFactory
    {
        public MessageViewmodel CreateMessageViewmodel(string message, bool localAuthor)
        {
            return new MessageViewmodel(message, localAuthor);
        }
    }
}