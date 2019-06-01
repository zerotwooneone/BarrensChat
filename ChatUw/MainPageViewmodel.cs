using System;
using System.Collections.ObjectModel;
using ChatUw.Message;

namespace ChatUw
{
    public class MainPageViewmodel
    {
        private readonly IMessageViewmodelFactory _messageViewmodelFactory;

        public MainPageViewmodel(ObservableCollection<MessageViewmodel> messages,
            IMessageViewmodelFactory messageViewmodelFactory)
        {
            _messageViewmodelFactory = messageViewmodelFactory;
            Messages = messages;
        }

        public ObservableCollection<MessageViewmodel> Messages { get; private set; }
    }
}