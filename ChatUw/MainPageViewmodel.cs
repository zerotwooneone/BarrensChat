using System;
using System.Collections.ObjectModel;
using ChatUw.Message;

namespace ChatUw
{
    public class MainPageViewmodel
    {
        private readonly IMessageViewmodelFactory _messageViewmodelFactory;

        public MainPageViewmodel(IMessageViewmodelFactory messageViewmodelFactory)
        {
            _messageViewmodelFactory = messageViewmodelFactory;
            Messages = new ObservableCollection<MessageViewmodel>
            {
                new MessageViewmodel("message 1", true),
                new MessageViewmodel("message 2", false)
            };
        }

        public ObservableCollection<MessageViewmodel> Messages { get; private set; }
    }
}