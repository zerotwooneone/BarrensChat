using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using ChatUw.Annotations;

namespace ChatUw
{
    public class ViewmodelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected ICommand CreateStandardUiCommend(Action onExecute)
        {
            return CreateStandardUiCommend((cmd, evtArgs) => onExecute());
        }
        protected ICommand CreateStandardUiCommend<T>(Action<T> onExecute)
        {
            return CreateStandardUiCommend((cmd, evtArgs) => onExecute((T)evtArgs.Parameter));
        }
        protected ICommand CreateStandardUiCommend(TypedEventHandler<XamlUICommand, ExecuteRequestedEventArgs> onExecute)
        {
            var command = new StandardUICommand();
            command.ExecuteRequested += onExecute;
            return command;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}