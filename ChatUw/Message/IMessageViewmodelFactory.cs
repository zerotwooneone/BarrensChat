namespace ChatUw.Message
{
    public interface IMessageViewmodelFactory
    {
        MessageViewmodel CreateMessageViewmodel(string message, bool localAuthor);
    }
}