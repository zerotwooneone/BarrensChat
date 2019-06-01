using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ChatUw.Message
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LocalAuthorTemplate { get; set; }
        public DataTemplate RemoteAuthorTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            var message = (MessageViewmodel) item;
            return message.LocalAuthor ? LocalAuthorTemplate : RemoteAuthorTemplate;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return this.SelectTemplateCore(item);
        }
    }
}