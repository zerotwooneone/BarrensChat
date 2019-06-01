namespace ChatUw.Message
{
    public class MessageViewmodel
    {
        public MessageViewmodel(string text, 
            bool localAuthor)
        {
            Text = text;
            LocalAuthor = localAuthor;
        }

        public string Text { get; }
        public bool LocalAuthor { get; }
    }
}
